using Albite.Reader.Core.Diagnostics;
using Albite.Reader.Speech.Narration.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Speech.Narration
{
    public abstract class Narrator<TLocation>
    {
        private static readonly string Tag = "Narrator";

        internal RootNode Root { get; private set; }

        private object myLock = new object();

        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        internal Narrator(RootNode root, NarrationSettings settings)
        {
            // Set up root node
            Root = root;
        }

        public TLocation Location
        {
            get
            {
                // TODO
                return default(TLocation);
            }
        }

        public void ReadAsync()
        {
            string text = Root.ToSsml();
            Log.I(Tag, text);
            synthesizer.SpeakSsmlAsync(text);
        }
    }
}
