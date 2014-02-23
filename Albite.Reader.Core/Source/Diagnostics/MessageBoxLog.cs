using System;
using System.Diagnostics;
using System.Windows;

namespace Albite.Reader.Core.Diagnostics
{
    internal class MessageBoxLog : AbstractLog
    {
        public override void Log(Level level, string tag, string message, Exception exception)
        {
            // Show only errors to the user
            if (level == Level.Error)
            {
                string errorMessage = message;

                if (exception != null)
                {
                    // Add info from exception if there's any
                    errorMessage = string.Format("{0}\n\n{1}\n\n{2}", message, exception.Message, exception.StackTrace);
                }

                MessageBox.Show(errorMessage, "An error has occurred", MessageBoxButton.OK);
            }
        }

        public override void Flush() { }

        public override void Dispose() { }
    }
}
