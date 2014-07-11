namespace Albite.Reader.Speech.Narration.Internal
{
    internal abstract class NarrationExpression : INarrationExpression
    {
        protected INarrationCommand Previous { get; protected set; }
        protected INarrationCommand Next { get; protected set; }

        public void Execute()
        {
            // TODO
        }
    }
}
