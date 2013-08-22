using System.Collections.Generic;
using GalaSoft.MvvmLight;
using JistBridge.Messages;
using JistBridge.UI.ReportView;

// ReSharper disable once CSharpWarnings::CS0665

namespace JistBridge.Data.ReST {
	public class GetReportResponse : ViewModelBase {
		private bool _reportVisible;

		public class Report {
			public class Metadata {
				public class Fields {
					public string dtg { get; set; }
					public string sourceSystem { get; set; }
					public string analyst { get; set; }
				}

				public string resourceId { get; set; }
				public string resourceField { get; set; }
				public string offsetField { get; set; }
				public string textField { get; set; }
				public Fields fields { get; set; }
			}

			public class Text {
				public int offset { get; set; }
				public string text { get; set; }
			}

			public Metadata metadata { get; set; }
			public List<Text> texts { get; set; }
			public object diagram { get; set; }
		}

		public int resultCode { get; set; }
		public string description { get; set; }
		public Report report { get; set; }

		public string ShortName {
			get { return report.metadata.fields.dtg; }
		}

		private ReportView ReportView { get; set; }

		public int TextsCount {
			get {
				return report.texts.Count;
			}
		} 

		public bool ReportVisible {
			get { return _reportVisible; }
			set {
				if (value == _reportVisible) {
					return;
				}
				if (_reportVisible = value) {
					if (ReportView == null)
					{
					    ReportView = new ReportView();
					    ReportView.ReportViewModel.GetReportResponse = this;
					}
				    new AddRemoveReportViewMessage(null, null) {
						Operation = Operation.Add,
						ReportView = ReportView,
						TabText = ReportView.ReportViewModel.GetReportResponse.ShortName
					}.Send();
				}
				else {
					new AddRemoveReportViewMessage(null, null) {
						Operation = Operation.Remove,
						ReportView = ReportView
					}.Send();
				}
				RaisePropertyChanged("ReportVisible");
			}
		}
	}
}