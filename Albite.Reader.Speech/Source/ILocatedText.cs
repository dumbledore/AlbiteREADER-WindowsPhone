namespace Albite.Reader.Speech
{
    public interface ILocatedText<TLocation>
    {
        string Text { get; }
        TLocation Location { get; }
    }
}
