using System;

namespace Albite.Reader.Core.Test
{
    public abstract class TestPrepareWrapper : TestCaseWrapper
    {
        public TestPrepareWrapper(TestCase test) : base(test) { }

        protected abstract void onTearUp();
        protected abstract void onTearDown();

        protected override void TestImplementation()
        {
            Exception exception = null;

            onTearUp();

            try
            {
                test.Test();
            }
            catch (Exception e)
            {
                exception = e;
            }

            onTearDown();

            if (exception != null)
            {
                throw exception;
            }
        }
    }
}
