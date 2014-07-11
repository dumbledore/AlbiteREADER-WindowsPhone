namespace Albite.Reader.Speech.Narration
{
    public interface INarrationState
    {
        string Language { get; set; }
        float Speed { get; set; }
    }
}
