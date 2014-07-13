namespace Albite.Reader.Speech.Narration.Nodes
{
    internal abstract class LanguageNode : NarrationNode
    {
        public string Language { get; set; }

        public LanguageNode(string language)
        {
            Language = language;
        }
    }
}
