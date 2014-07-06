namespace Albite.Reader.Speech.Narration
{
    public abstract class NarrationExpression : INarrationCommand
    {
        public INarrationCommand Previous { get; protected set; }
        public INarrationCommand Next { get; protected set; }

        public void Execute()
        {
            // TODO
        }
    }
}
