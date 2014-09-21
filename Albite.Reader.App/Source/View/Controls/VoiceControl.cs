using Albite.Reader.Speech.Narration;
using System.Windows;

namespace Albite.Reader.App.View.Controls
{
    public class VoiceControl : HeaderedContentControl
    {
        public static readonly DependencyProperty VoiceProperty
            = DependencyProperty.Register("Voice", typeof(NarrationVoice), typeof(VoiceControl),
            new PropertyMetadata(onVoiceChanged));

        public NarrationVoice Voice
        {
            get { return (NarrationVoice)GetValue(VoiceProperty); }
            set { SetValue(VoiceProperty, value); }
        }

        private static void onVoiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            VoiceControl control = (VoiceControl)d;
            NarrationVoice voice = (NarrationVoice)e.NewValue;

            // Set voice name
            control.HeaderText = voice.Name;
            control.ContentText = string.Format("{0}, {1}", voice.Language, voice.Male ? "male" : "female");
        }
    }
}
