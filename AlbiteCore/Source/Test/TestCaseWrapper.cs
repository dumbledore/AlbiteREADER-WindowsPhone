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
