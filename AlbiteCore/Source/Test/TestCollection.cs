﻿using System.Collections.Generic;

namespace SvetlinAnkov.Albite.Core.Test
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