using System.Windows;
using System.Windows.Controls;

namespace JistBridge.UI.MainWindow
{

    class PanesStyleSelector : StyleSelector
    {
        public Style ProjectInstanceStyle
        {
            get;
            set;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return ProjectInstanceStyle;
        }
    }

}