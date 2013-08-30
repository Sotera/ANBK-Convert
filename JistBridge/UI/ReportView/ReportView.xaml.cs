using System.ComponentModel.Composition;
using System.Windows.Controls;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.ReportView {
	/// <summary>
	/// Interaction logic for ReportView.xaml
	/// </summary>
	public partial class ReportView:UserControl {
		public ReportView() {
			InitializeComponent();

			new QueueMefComposeMessage(null, null, this, null).Send();
		}

		[Import]
		public IReportViewModel ReportViewModel {
			set { DataContext = value; }
			get {
				var retVal = DataContext as ReportViewModel;
				return retVal;
			}
		}

		public string ShortName {
			get {
				return (ReportViewModel != null && ReportViewModel.GetReportResponse != null)
					? ReportViewModel.GetReportResponse.ShortName
					: "";
			}
		}
	}
}