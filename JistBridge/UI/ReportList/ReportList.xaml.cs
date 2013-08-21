using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.ReportList {
	public partial class ReportList {
		[Import(typeof(IReportListViewModel ))]
		internal IReportListViewModel ReportListViewModel {
			set {
				DataContext = value;
			}
		}

		public ReportList() {
			InitializeComponent();
			new QueueMefComposeMessage(null, null, this, null).Send();
		}
	}
}