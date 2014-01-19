using System.Windows;

namespace SvetlinAnkov.Albite.READER.View.Controls
{
    public class TocControl : HierarchicalTextControl
    {
        public static readonly DependencyProperty LocationProperty
            = DependencyProperty.Register("Location", typeof(string), typeof(TocControl), null);

        public string Location
        {
            get { return (string)GetValue(LocationProperty); }
            set { SetValue(LocationProperty, value); }
        }
    }
}
