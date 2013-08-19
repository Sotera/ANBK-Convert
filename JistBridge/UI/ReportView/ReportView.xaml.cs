using System.ComponentModel.Composition;
using System.Windows.Controls;
using JistBridge.Interfaces;

namespace JistBridge.UI.ReportView
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView : UserControl
    {
        public ReportView()
        {
            InitializeComponent();
        }

        [Import]
        public IReportViewModel RichTextBoxViewModel
        {
            set
            {
                DataContext = value;
            }
            get
            {
                if (!(DataContext is ReportViewModel))
                    return null;
                return DataContext as ReportViewModel;
            }
        }
    }
}
