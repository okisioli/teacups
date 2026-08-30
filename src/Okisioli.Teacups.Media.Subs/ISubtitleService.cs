namespace Okisioli.Teacups.Media.Subs;

/// <summary>Loads and plays SRT subtitles while reporting the active line.</summary>
public interface ISubtitleService : IDisposable
{
    /// <summary>Gets the text of the active subtitle line, if any.</summary>
    string? CurrentText { get; }

    /// <summary>Gets whether subtitle playback is active.</summary>
    bool IsPlaying { get; }

    /// <summary>Gets the current playback position.</summary>
    TimeSpan Position { get; }

    /// <summary>Gets the duration of the loaded subtitle file.</summary>
    TimeSpan Duration { get; }

    /// <summary>Occurs when the active subtitle line changes.</summary>
    event EventHandler<SubtitleChangedEventArgs>? CurrentLineChanged;

    /// <summary>Loads subtitles from an SRT file.</summary>
    /// <param name="filePath">The path to the SRT file.</param>
    /// <param name="cancellationToken">A token that cancels the load operation.</param>
    Task LoadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>Starts subtitle playback.</summary>
    void Play();

    /// <summary>Pauses subtitle playback.</summary>
    void Pause();
}
