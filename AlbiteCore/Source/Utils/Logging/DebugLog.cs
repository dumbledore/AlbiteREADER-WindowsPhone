using System;
using System.Diagnostics;

namespace SvetlinAnkov.Albite.Core.Utils.Logging
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
