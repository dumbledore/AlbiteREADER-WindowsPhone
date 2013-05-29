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
        Popup popup;

        public WaitPopup()
        {
            Size size = Application.Current.RootVisual.RenderSize;

            WaitControl waitControl = new WaitControl();
            waitControl.Width = size.Width;
            waitControl.Height = size.Height;

            popup = new Popup();
            popup.Child = waitControl;
        }

        public bool IsOpen
        {
            get { return popup.IsOpen; }
            set
            {
                hideBars();
                popup.IsOpen = value;
            }
        }

        private void hideBars()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                // Setting SystemTray.IsVisible causes exceptions in design mode,
                // so skipping it, if used in VS, Expression Blend, etc.
                // For more info, check: http://stackoverflow.com/a/834332/348183
                return;
            }

            // Hide the top bar
            SystemTray.IsVisible = false;

            // Hide the bottom bar
            IApplicationBar bar = AppUtils.ApplicationBar;
            if (bar != null)
            {
                bar.IsVisible = false;
            }
        }
    }
}
