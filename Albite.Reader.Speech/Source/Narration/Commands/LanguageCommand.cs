namespace Albite.Reader.Speech.Narration.Commands
{
    public class LanguageCommand : NarrationCommand
    {
        public string Language { get; private set; }

        public LanguageCommand(string language)
        {
            Language = language;
        }

        public override void Execute(INarrationEngine engine)
        {
            engine.State.Language = Language;
        }

        public override string ToString()
        {
            return "Language: {{" + Language + "}}";
        }
    }
}
