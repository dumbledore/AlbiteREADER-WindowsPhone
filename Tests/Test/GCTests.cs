using System;
using System.Net;
using System.Windows;
using SvetlinAnkov.Albite.Core.Test;

namespace SvetlinAnkov.Albite.Tests.Test
{
    public class GCTests : TestCollection
    {
        private static readonly int iterations = 10000;
        private static readonly int gcIterations = iterations / 1000;

        protected override System.Collections.Generic.ICollection<TestCase> CreateTests()
        {
            return new TestCase[]
            {
                new GCSimpleTest(iterations, gcIterations),
                new GCCachedTest(iterations, gcIterations),
            };
        }

        private abstract class GCTest : TestCase
        {
            protected readonly int iterations;
            protected readonly int gcIteration;

            public GCTest(int iterations, int gcIteration)
            {
                this.iterations = iterations;
                this.gcIteration = gcIteration;
            }

            protected override void TestImplementation()
            {
                string[] s = null;
                InitGarbage(ref s);

                for (int i = 0; i < iterations; i++)
                {
                    if (i % gcIteration == 0)
                    {
                        GC.Collect();
                    }

                    MakeGarbage(ref s);
                }
            }

            protected string MakeString()
            {
                return DateTime.Now.Ticks.ToString();
            }

            protected abstract void InitGarbage(ref string[] s);
            protected abstract void MakeGarbage(ref string[] s);
        }

        private class GCSimpleTest : GCTest
        {
            public GCSimpleTest(int iterations, int gcIteration)
                : base(iterations, gcIteration) { }

            protected override void InitGarbage(ref string[] s)
            {
                s = null;
            }

            protected override void MakeGarbage(ref string[] s)
            {
                s = new string[] { MakeString() };
            }
        }

        private class GCCachedTest : GCTest
        {
            public GCCachedTest(int iterations, int gcIteration)
                : base(iterations, gcIteration) { }

            protected override void InitGarbage(ref string[] s)
            {
                s = new string[1];
            }

            protected override void MakeGarbage(ref string[] s)
            {
                s[0] = MakeString();
            }
        }
    }
}
