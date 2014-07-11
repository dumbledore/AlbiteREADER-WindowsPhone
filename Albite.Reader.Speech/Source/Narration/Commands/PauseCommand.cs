namespace Albite.Reader.Speech.Narration.Commands
{
    public class PauseCommand : NarrationCommand
    {
        public int Duration { get; private set; }

        public PauseCommand(int duration)
        {
            Duration = duration;
        }

        public override void Execute(INarrationEngine engine)
        {
            engine.Pause(Duration);
        }

        public override string ToString()
        {
            return "Pause: {{" + Duration + "}}";
        }
    }
}
