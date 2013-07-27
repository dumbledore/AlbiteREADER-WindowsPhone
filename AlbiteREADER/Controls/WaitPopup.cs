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
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace SvetlinAnkov.Albite.READER.Controls
{

    public class WaitPopup
    {
        public static readonly int Minimum = 0;
        public static readonly int Maximum = 100;

        Popup popup;
        WaitControl control;

        public WaitPopup()
        {
            Size size = Application.Current.RootVisual.RenderSize;

            control = new WaitControl();
            control.Width = size.Width;
            control.Height = size.Height;
            control.ProgressBar.Maximum = 100;
            control.ProgressBar.Value = 0;

            popup = new Popup();
            popup.Child = control;
        }

        private int progress;
        public int Progress
        {
            get { return progress; }
            set
            {
                // Clamp it
                progress = value < Minimum
                    ? Minimum
                    : value > Maximum ? Maximum : value;

                // Update the progress bar
                control.ProgressBar.Value = progress;
            }
        }

        public void Start()
        {
            Progress = 0;
            popup.IsOpen = true;
        }

        public void Finish()
        {
            Progress = 100;
            popup.IsOpen = false;
        }
    }
}
