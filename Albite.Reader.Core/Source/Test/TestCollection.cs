using System.Collections.Generic;

namespace Albite.Reader.Core.Test
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

        public override void Dispose()
        {
            foreach (TestCase test in tests)
            {
                test.Dispose();
            }
        }
    }
}
