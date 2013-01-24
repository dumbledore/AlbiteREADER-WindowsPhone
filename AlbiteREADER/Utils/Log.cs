using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SvetlinAnkov.AlbiteREADER.Utils
{
    public static class Log
    {
        public enum Level { Error, Warning, Info, Debug }

        public static void E(string tag, string message)
        {
            log(Level.Error, tag, message);
        }

        public static void E(string tag, string message, Exception exception)
        {
            log(Level.Error, tag, message, exception);
        }

        public static void W(string tag, string message)
        {
            log(Level.Warning, tag, message);
        }

        public static void W(string tag, string message, Exception exception)
        {
            log(Level.Warning, tag, message, exception);
        }

        public static void I(string tag, string message)
        {
            log(Level.Info, tag, message);
        }

        public static void I(string tag, string message, Exception exception)
        {
            log(Level.Info, tag, message, exception);
        }

        [Conditional("DEBUG")]
        public static void D(string tag, string message)
        {
            log(Level.Debug, tag, message);
        }

        [Conditional("DEBUG")]
        public static void D(string tag, string message, Exception exception)
        {
            log(Level.Debug, tag, message, exception);
        }

        // TODO: What about sending the logs from the file in an email?

        private static void log(Level level, string tag, string message)
        {
            log(level, tag, message, null);
        }

        private static void log(Level level, string tag, string message, Exception exception)
        {
            string output = string.Format("{0} {1}/{2}: {3} {4} {5}",
                DateTime.Now, level, tag, message,
                exception == null ? null : exception.Message,
                exception == null ? null : exception.StackTrace);
#if DEBUG
            Debug.WriteLine(output);
#else
            // TODO: Log to file
#endif
        }
    }
}
