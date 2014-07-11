using Albite.Reader.Speech.Narration.Commands;

namespace Albite.Reader.Speech.Narration
{
    public interface INarrationEngine
    {
        INarrationState State { get; }
        void Read(string text);
        void Pause(int duration);
    }
}
