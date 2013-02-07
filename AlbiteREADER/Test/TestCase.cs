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

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public abstract class TestCase
    {
        public bool Passed { get; private set; }

        private readonly string tag;

        public TestCase()
        {
            Passed = false;
            tag = GetType().Name;
        }

        public void Test()
        {
            Log(@"/== Running ==\");

            TestImplementation();
            Passed = true;

            Log("\\==  Passed ==/\n");
        }

        protected void Log(string message)
        {
            Utils.Log.D(tag, message);
        }

        protected void Log(string format, params object[] args)
        {
            Utils.Log.D(tag, string.Format(format, args));
        }


        protected abstract void TestImplementation();
    }
}
