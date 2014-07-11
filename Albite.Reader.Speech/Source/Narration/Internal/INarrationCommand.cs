namespace Albite.Reader.Speech.Narration.Internal
{
    internal interface INarrationCommand
    {
        INarrationCommand Previous { get; }
        INarrationCommand Next { get; }
        void Execute();
    }
}
