namespace Olivercode.Media.Subs;

public sealed class SubtitleChangedEventArgs(string? text) : EventArgs
{
    public string? Text { get; } = text;
}
