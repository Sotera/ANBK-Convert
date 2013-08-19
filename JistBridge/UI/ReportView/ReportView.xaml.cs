using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.ReportView
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class ReportView
    {
        public ReportView()
        {
            InitializeComponent();

            new QueueMefComposeMessage(null, null, this, null).Send();
        }

        [Import]
        public IReportViewModel ReportViewModel
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
