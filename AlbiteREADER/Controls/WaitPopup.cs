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
            set { popup.IsOpen = value; }
        }
    }
}
