using System;

namespace SvetlinAnkov.Albite.Core.Test
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

            DateTime start = DateTime.Now;
            TestImplementation();
            DateTime end = DateTime.Now;

            Passed = true;

            Log("Test took {0} ms", end.Subtract(start).TotalMilliseconds);
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

        protected void Assert(bool expression)
        {
            if (!expression)
            {
                throw new InvalidOperationException();
            }
        }

        protected void Assert(object o)
        {
            Assert(o != null);
        }

        protected abstract void TestImplementation();

        public virtual int NumberOfTests { get { return 1; } }
    }
}
