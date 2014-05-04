using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Albite.Reader.App.View.Controls
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

        public Color BackgroundColor
        {
            set
            {
                LayoutRoot.Background = new SolidColorBrush(value);
            }
        }

        public Color ForegroundColor
        {
            set
            {
                Brush brush = new SolidColorBrush(value);
                ProgressBar.Foreground = brush;
                LoadingText.Foreground = brush;
            }
        }

        private bool isIndeterminate_ = false;
        public bool IsIndeterminate
        {
            get { return isIndeterminate_; }
            set
            {
                isIndeterminate_ = value;
                ProgressBar.IsIndeterminate = value;
            }
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

            // Update the control using the cached value
            ProgressBar.IsIndeterminate = IsIndeterminate;

            Visibility = Visibility.Visible;
        }

        public void Finish()
        {
            // Unset IsIndeterminate *directly* on the control,
            // and leave the cached value untouched.
            ProgressBar.IsIndeterminate = false;
            Progress = Maximum;
            Visibility = Visibility.Collapsed;
        }
    }
}
