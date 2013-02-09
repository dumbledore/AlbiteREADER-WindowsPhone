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
using System.Collections.Generic;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public abstract class TestCollection : TestCase
    {
        private ICollection<TestCase> tests;
        protected abstract ICollection<TestCase> CreateTests();

        public TestCollection()
        {
            tests = CreateTests();
        }

        protected override void TestImplementation()
        {
            foreach (TestCase test in tests)
            {
                test.Test();
            }
        }

        public override int NumberOfTests
        {
            get
            {
                int count = 0;
                foreach (TestCase test in tests)
                {
                    count += test.NumberOfTests;
                }

                return count;
            }
        }
    }
}
