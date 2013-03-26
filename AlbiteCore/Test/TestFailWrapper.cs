using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SvetlinAnkov.Albite.Core.Test
{
    public class TestFailWrapper : TestCaseWrapper
    {
        public TestFailWrapper(TestCase test) : base(test) { }

        protected override void TestImplementation()
        {
            try
            {
                test.Test();
            }
            catch (Exception e)
            {
                // All is fine, there was an error.
                Log("Successfully threw an exception: " + e.Message
                    + (e.InnerException != null ? " -> " + e.InnerException.Message : ""));
                return;
            }

            throw new Exception("The test didn't throw an exception when expected to");
        }
    }
}
