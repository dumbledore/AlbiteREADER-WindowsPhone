using System;
namespace Albite.Reader.Speech.Narration.Commands
{
    public abstract class NarrationCommand
    {
        public NarrationCommand Previous { get; private set; }
        public NarrationCommand Next { get; private set; }
        public abstract void Execute(INarrationEngine engine);

        public void AddNext(NarrationCommand nextCommand)
        {
            if (Next != null)
            {
                throw new InvalidOperationException("Already added");
            }

            Next = nextCommand;
            nextCommand.Previous = this;
        }
    }
}
