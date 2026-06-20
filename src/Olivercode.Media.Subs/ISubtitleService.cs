namespace Olivercode.Media.Subs;

public interface ISubtitleService : IDisposable
{
    string? CurrentText { get; }

    bool IsPlaying { get; }

    TimeSpan Position { get; }

    TimeSpan Duration { get; }

    event EventHandler<SubtitleChangedEventArgs>? CurrentLineChanged;

    Task LoadAsync(string filePath, CancellationToken cancellationToken = default);

    void Play();

    void Pause();
}
