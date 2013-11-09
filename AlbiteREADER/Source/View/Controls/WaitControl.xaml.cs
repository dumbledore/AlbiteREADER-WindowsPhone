using System.Windows;
using System.Windows.Controls;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public partial class WaitControl : UserControl
    {
        public WaitControl()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return LoadingText.Text; }
            set { LoadingText.Text = value; }
        }

        public bool IsIndeterminate
        {
            get { return ProgressBar.IsIndeterminate; }
            set { ProgressBar.IsIndeterminate = value; }
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
