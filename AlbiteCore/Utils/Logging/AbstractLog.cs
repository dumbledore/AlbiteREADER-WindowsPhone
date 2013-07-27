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

namespace SvetlinAnkov.Albite.Core.Utils.Logging
{
    internal abstract class AbstractLog : IDisposable
    {
        public abstract void Log(Log.Level level, string tag, string message, Exception exception);

        protected static string LogAsString(Log.Level level, string tag, string message, Exception exception)
        {
            DateTime now = DateTime.Now;

            return string.Format("{0}.{1} {2}/{3}: {4} {5} {6}",
                            now, now.Millisecond, level, tag, message,
                            exception == null ? null : exception.Message,
                            exception == null ? null : exception.StackTrace);
        }

        public abstract void Flush();

        public abstract void Dispose();
    }
}
