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

namespace SvetlinAnkov.AlbiteREADER.Utils.Logging
{
    internal abstract class AbstractLog : IDisposable
    {
        public abstract void Log(Log.Level level, string tag, string message, Exception exception);

        protected static string LogAsString(Log.Level level, string tag, string message, Exception exception)
        {
            return string.Format("{0} {1}/{2}: {3} {4} {5}",
                            DateTime.Now, level, tag, message,
                            exception == null ? null : exception.Message,
                            exception == null ? null : exception.StackTrace);
        }

        public abstract void Flush();

        public abstract void Dispose();
    }
}
