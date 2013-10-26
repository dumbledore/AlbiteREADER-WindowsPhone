using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public partial class WaitControl : UserControl
    {
        public WaitControl()
        {
            InitializeComponent();
        }

        public double Progress
        {
            get { return ProgressBar.Value; }
            set { ProgressBar.Value = value; }
        }

        public double Minimum
        {
            get { return ProgressBar.Minimum; }
            set { ProgressBar.Minimum = value; }
        }

        public double Maximum
        {
            get { return ProgressBar.Maximum; }
            set { ProgressBar.Maximum = value; }
        }

        public void Start()
        {
            Progress = Minimum;
            Visibility = Visibility.Visible;
        }

        public void Finish()
        {
            Progress = Maximum;
            Visibility = Visibility.Collapsed;
        }
    }
}
