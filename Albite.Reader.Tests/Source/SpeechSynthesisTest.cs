﻿using Microsoft.Phone.Shell;
using Albite.Reader.Core.Test;
using Windows.Foundation;
using Windows.Phone.Speech.Synthesis;

namespace Albite.Reader.Tests
{
    class SpeechSynthesisTest : TestCase
    {
        protected override void TestImplementation()
        {
            // Disable idle mode so the app will
            // run under locked screen
            PhoneApplicationService.Current.ApplicationIdleDetectionMode
                = IdleDetectionMode.Disabled;

            // Speak it
            SpeechSynthesizer synth = new SpeechSynthesizer();
            IAsyncAction action = synth.SpeakTextAsync("“Curiouser and curiouser!” cried Alice (she was so much surprised, that for the moment she quite forgot how to speak good English); “now I'm opening out like the largest telescope that ever was! Good-bye, feet!” (for when she looked down at her feet, they seemed to be almost out of sight, they were getting so far off).");
            action.GetResults();
        }
    }
}
