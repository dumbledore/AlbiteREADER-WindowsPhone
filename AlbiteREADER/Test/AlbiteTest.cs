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
using System.Diagnostics;

namespace SvetlinAnkov.AlbiteREADER.Test
{
    public abstract class AlbiteTest
    {
        public bool Passed { get; private set; }

        public AlbiteTest()
        {
            Passed = false;
        }

        public void Test()
        {
            Debug.WriteLine("=== Running {0} ===", this.GetType().Name);
            TestImplementation();
            Passed = true;
            Debug.WriteLine("{0} Passed\n", this.GetType().Name);
        }

        protected abstract void TestImplementation();
    }
}
