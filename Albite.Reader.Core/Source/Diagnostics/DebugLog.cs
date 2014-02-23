using System;
using System.Diagnostics;

namespace Albite.Reader.Core.Diagnostics
{
    internal class DebugLog : AbstractLog
    {
        public override void Log(Level level, string tag, string message, Exception exception)
        {
            Debug.WriteLine(LogAsString(level, tag, message, exception));
        }

        public override void Flush() { }

        public override void Dispose() { }
    }
}
