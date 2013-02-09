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

namespace SvetlinAnkov.Albite.Tests.Test.Model.Container
{
    public class InvalidEpubContainerTest : EpubContainerTest
    {
        /// <summary>
        /// Test an invalid epub container.
        /// The test will fail if the epub container doesn't throw an exception
        /// </summary>
        /// <param name="epubPath">Path to the epub resource</param>
        public InvalidEpubContainerTest(string epubPath) : base(epubPath) { }

        protected override void TestImplementation()
        {
            try
            {
                base.TestImplementation();
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
