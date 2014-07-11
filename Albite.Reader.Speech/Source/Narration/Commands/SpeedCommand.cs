using System;

namespace Albite.Reader.Speech.Narration.Commands
{
    public class SpeedCommand : NarrationCommand
    {
        public float Speed { get; private set; }

        public SpeedCommand(float speed)
        {
            Speed = speed;
        }

        public override void Execute(INarrationEngine engine)
        {
            engine.State.Speed = Speed;
        }

        public override string ToString()
        {
            return "Speed: {{" + Speed + "}}";
        }
    }
}
