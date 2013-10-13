using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SvetlinAnkov.Albite.Core.Test
{
    public abstract class TestCaseWrapper : TestCase
    {
        protected readonly TestCase test;

        public TestCaseWrapper(TestCase test)
        {
            this.test = test;
        }
    }
}
