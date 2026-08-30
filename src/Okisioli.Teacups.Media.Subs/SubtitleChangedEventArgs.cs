namespace Okisioli.Teacups.Media.Subs;

/// <summary>Contains the text for a changed subtitle line.</summary>
/// <param name="text">The new subtitle text, or <see langword="null"/> when no cue is active.</param>
public sealed class SubtitleChangedEventArgs(string? text) : EventArgs
{
    /// <summary>Gets the new subtitle text, or <see langword="null"/> when no cue is active.</summary>
    public string? Text { get; } = text;
}
