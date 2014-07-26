namespace Albite.Reader.Core.Test
{
    public abstract class TestCaseWrapper : TestCase
    {
        protected readonly TestCase test;

        public TestCaseWrapper(TestCase test)
        {
            this.test = test;
        }

        public override void Dispose()
        {
            test.Dispose();
        }
    }
}
