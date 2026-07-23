using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

namespace Teacups.Media.Subs;

public sealed class SubtitleService : ISubtitleService
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(33);

    private static readonly Regex CuePattern = new(
        @"(?:\A|\r?\n\r?\n)\s*\d+\s*\r?\n" +
        @"(?<start>\d{2,}:\d{2}:\d{2}[,.]\d{3})\s*-->\s*" +
        @"(?<end>\d{2,}:\d{2}:\d{2}[,.]\d{3})[^\r\n]*\r?\n" +
        @"(?<text>.*?)(?=\r?\n\r?\n|\z)",
        RegexOptions.Compiled | RegexOptions.Singleline);

    private static readonly Regex TimestampPattern = new(
        @"^(?<hours>\d+):(?<minutes>\d{2}):(?<seconds>\d{2})[,.](?<milliseconds>\d{3})$",
        RegexOptions.Compiled);

    private static readonly Regex FormattingTagPattern = new(
        @"<[^>]+>",
        RegexOptions.Compiled);

    private readonly Lock stateLock = new();
    private readonly Stopwatch playbackClock = new();
    private readonly Timer timer;
    private IReadOnlyList<SubtitleCue> cues = [];
    private TimeSpan position;
    private bool isLoaded;
    private bool isDisposed;

    public SubtitleService()
    {
        timer = new Timer(OnTimerTick);
    }

    public string? CurrentText { get; private set; }

    public bool IsPlaying { get; private set; }

    public TimeSpan Position
    {
        get
        {
            lock (stateLock)
                return GetPosition();
        }
    }

    public TimeSpan Duration { get; private set; }

    public event EventHandler<SubtitleChangedEventArgs>? CurrentLineChanged;

    public async Task LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        if (!File.Exists(filePath))
            throw new FileNotFoundException("Subtitle file not found.", filePath);

        string content = await File.ReadAllTextAsync(filePath, cancellationToken);
        IReadOnlyList<SubtitleCue> loadedCues = Parse(content);
        SubtitleChangedEventArgs? eventArgs;

        lock (stateLock)
        {
            StopPlayback();
            cues = loadedCues;
            position = TimeSpan.Zero;
            Duration = cues.Count == 0 ? TimeSpan.Zero : cues.Max(cue => cue.End);
            isLoaded = true;
            eventArgs = SetCurrentText(FindText(position));
        }

        RaiseCurrentLineChanged(eventArgs);
    }

    public void Play()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        lock (stateLock)
        {
            if (!isLoaded)
                throw new InvalidOperationException("Load a subtitle file before starting playback.");

            if (IsPlaying || Duration == TimeSpan.Zero)
                return;

            if (position >= Duration)
                position = TimeSpan.Zero;

            playbackClock.Restart();
            IsPlaying = true;
            timer.Change(TimeSpan.Zero, UpdateInterval);
        }
    }

    public void Pause()
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);

        SubtitleChangedEventArgs? eventArgs;
        lock (stateLock)
        {
            position = GetPosition();
            StopPlayback();
            eventArgs = SetCurrentText(FindText(position));
        }

        RaiseCurrentLineChanged(eventArgs);
    }

    public void Dispose()
    {
        lock (stateLock)
        {
            if (isDisposed)
                return;

            isDisposed = true;
            StopPlayback();
            timer.Dispose();
        }
    }

    private void OnTimerTick(object? state)
    {
        SubtitleChangedEventArgs? eventArgs;

        lock (stateLock)
        {
            if (!IsPlaying)
                return;

            TimeSpan currentPosition = GetPosition();
            if (currentPosition >= Duration)
            {
                position = Duration;
                StopPlayback();
            }

            eventArgs = SetCurrentText(FindText(currentPosition));
        }

        RaiseCurrentLineChanged(eventArgs);
    }

    private TimeSpan GetPosition()
    {
        if (!IsPlaying)
            return position;

        TimeSpan currentPosition = position + playbackClock.Elapsed;
        return currentPosition > Duration ? Duration : currentPosition;
    }

    private void StopPlayback()
    {
        timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        playbackClock.Reset();
        IsPlaying = false;
    }

    private string? FindText(TimeSpan currentPosition)
    {
        int low = 0;
        int high = cues.Count - 1;
        int candidate = -1;

        while (low <= high)
        {
            int middle = low + ((high - low) / 2);
            if (cues[middle].Start <= currentPosition)
            {
                candidate = middle;
                low = middle + 1;
            }
            else
            {
                high = middle - 1;
            }
        }

        return candidate >= 0 && currentPosition < cues[candidate].End
            ? cues[candidate].Text
            : null;
    }

    private SubtitleChangedEventArgs? SetCurrentText(string? text)
    {
        if (string.Equals(CurrentText, text, StringComparison.Ordinal))
            return null;

        CurrentText = text;
        return new SubtitleChangedEventArgs(text);
    }

    private void RaiseCurrentLineChanged(SubtitleChangedEventArgs? eventArgs)
    {
        if (eventArgs is not null)
            CurrentLineChanged?.Invoke(this, eventArgs);
    }

    private static IReadOnlyList<SubtitleCue> Parse(string content)
    {
        var result = new List<SubtitleCue>();

        foreach (Match match in CuePattern.Matches(content))
        {
            TimeSpan start = ParseTimestamp(match.Groups["start"].Value);
            TimeSpan end = ParseTimestamp(match.Groups["end"].Value);
            if (end < start)
                throw new InvalidDataException("An SRT cue ends before it starts.");

            string text = FormattingTagPattern.Replace(match.Groups["text"].Value, string.Empty)
                .Replace("\r\n", "\n", StringComparison.Ordinal)
                .Trim();

            result.Add(new SubtitleCue(start, end, WebUtility.HtmlDecode(text)));
        }

        if (result.Count == 0 && !string.IsNullOrWhiteSpace(content))
            throw new InvalidDataException("The file does not contain valid SRT subtitle cues.");

        return result.OrderBy(cue => cue.Start).ToArray();
    }

    private static TimeSpan ParseTimestamp(string value)
    {
        Match match = TimestampPattern.Match(value);
        if (!match.Success)
            throw new InvalidDataException($"Invalid SRT timestamp '{value}'.");

        return TimeSpan.FromHours(Parse("hours"))
            + TimeSpan.FromMinutes(Parse("minutes"))
            + TimeSpan.FromSeconds(Parse("seconds"))
            + TimeSpan.FromMilliseconds(Parse("milliseconds"));

        int Parse(string groupName) =>
            int.Parse(match.Groups[groupName].Value, CultureInfo.InvariantCulture);
    }

    private sealed record SubtitleCue(TimeSpan Start, TimeSpan End, string Text);
}
