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

namespace SvetlinAnkov.AlbiteREADER.Utils.Logging
{
    internal class DebugLog : AbstractLog
    {
        public override void Log(Log.Level level, string tag, string message, Exception exception)
        {
            Debug.WriteLine(LogAsString(level, tag, message, exception));
        }

        public override void Flush() { }

        public override void Dispose() { }
    }
}
