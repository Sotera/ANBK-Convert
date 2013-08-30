using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.ReportList {
	[Export(typeof (IReportListViewModel))]
	internal class ReportListViewModel : ViewModelBase, IReportListViewModel {
		private ObservableCollection<GetReportResponse> _reports;
		private GetReportResponse _selectedReport;

        public string Title{get { return "Reports"; }}

		internal ReportListViewModel() {
			ReportReceivedMessage.Register(this, msg => Reports.Add(msg.GetReportResponse));
		}

		public ObservableCollection<GetReportResponse> Reports {
			get { return _reports ?? (_reports = new ObservableCollection<GetReportResponse>()); }
		}

		public GetReportResponse SelectedReport {
			get { return _selectedReport; }
			set {
				_selectedReport = value;
				new SetPropertyEditorTargetMessage(this, null) {PropertiesObject = _selectedReport}.Send();
				RaisePropertyChanged("SelectedReport");
			}
		}
	}
}