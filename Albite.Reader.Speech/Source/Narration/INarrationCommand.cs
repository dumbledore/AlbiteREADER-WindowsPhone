namespace Albite.Reader.Speech.Narration
{
    public interface INarrationCommand
    {
        INarrationCommand Previous { get; }
        INarrationCommand Next { get; }
        void Execute();
    }
}
