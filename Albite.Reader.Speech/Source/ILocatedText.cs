namespace Albite.Reader.Speech
{
    public interface ILocatedText<TLocation>
    {
        int Id { get; }
        string Text { get; }
        TLocation Location { get; }
    }
}
