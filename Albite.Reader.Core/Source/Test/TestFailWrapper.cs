using System;

namespace Albite.Reader.Core.Test
{
    public class TestFailWrapper : TestCaseWrapper
    {
        private Type exceptionType;

        public TestFailWrapper(TestCase test, Type exceptionType = null)
            : base(test)
        {
            this.exceptionType = exceptionType;
        }

        protected override void TestImplementation()
        {
            try
            {
                test.Test();
            }
            catch (Exception e)
            {
                if (exceptionType != null && exceptionType != e.GetType())
                {
                    throw new Exception("Expected " + exceptionType.Name + " but got " + e.GetType().Name);
                }

                // All is fine, there was an error.
                Log("Successfully threw " + e.GetType().Name + ": " + e.Message
                    + (e.InnerException != null ? " -> " + e.InnerException.Message : ""));
                return;
            }

            throw new Exception("The test didn't throw an exception when expected to");
        }
    }
}
