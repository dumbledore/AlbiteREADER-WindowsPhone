﻿using System;
using System.Diagnostics;

namespace Albite.Reader.Core.Diagnostics
{
    public static class Log
    {
        private static readonly AbstractLog logger = createLog();

        public static string LogLocation
        {
            get
            {
                return IsolatedStorageLog.DefaultLogLocation;
            }
        }

        public static void E(string tag, string message)
        {
            log(Level.Error, tag, message);
        }

        public static void E(string tag, string message, Exception exception)
        {
            log(Level.Error, tag, message, exception);
            Flush();
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

        public static void Flush()
        {
            logger.Flush();
        }

        private static void log(Level level, string tag, string message)
        {
            log(level, tag, message, null);
        }

        private static void log(Level level, string tag, string message, Exception exception)
        {
            logger.Log(level, tag, message, exception);
        }

        private static AbstractLog createLog()
        {
#if DEBUG
            return new DebugLog();
#else
            return new MessageBoxLog();
#endif
        }
    }
}
