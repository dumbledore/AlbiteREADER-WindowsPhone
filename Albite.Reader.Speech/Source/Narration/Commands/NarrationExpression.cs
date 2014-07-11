namespace Albite.Reader.Speech.Narration.Commands
{
    public abstract class NarrationExpression : NarrationCommand
    {
        public string Text { get; private set; }

        public NarrationExpression(string text)
        {
            Text = text;
        }

        public override void Execute(INarrationEngine engine)
        {
            engine.Read(Text);
        }

        public override string ToString()
        {
            return "Expression: {{" + Text + "}}";
        }
    }
}
