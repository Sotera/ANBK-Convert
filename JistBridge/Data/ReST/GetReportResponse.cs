using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using JistBridge.Data.Model;
using JistBridge.Messages;
using JistBridge.UI.ReportView;
using Newtonsoft.Json;

// ReSharper disable once CSharpWarnings::CS0665

namespace JistBridge.Data.ReST {
	public class GetReportResponse : ViewModelBase {
		private bool _reportVisible;

		public class CReport {
			public class CMetadata {
				public string resourceId { get; set; }
				public string resourceField { get; set; }
				public string offsetField { get; set; }
				public string textField { get; set; }
				
			}

			public class CText {
				public int offset { get; set; }
				public string text { get; set; }
			}

			public CMetadata metadata { get; set; }
			public List<CText> texts { get; set; }
			public object diagram { get; set; }
			public Markup Markup { get; set; }
            public Dictionary<string,string>fields { get; set; } 
		}

		public int resultCode { get; set; }
		public string description { get; set; }

		public class ReportTextsAndMetadata {
			public List<CReport.CText> texts { get; set; }
			public CReport.CMetadata metadata { get; set; }
            public Dictionary<string, string> fields { get; set; }
		}

		public string Report {
			set {
				var textsAndMetadata = JsonConvert.DeserializeObject<ReportTextsAndMetadata>(value);
				//Make the XML pretty for easy reading
				foreach (var text in textsAndMetadata.texts) {
					var xmlDoc = XDocument.Parse(text.text);
					var prettyXml = xmlDoc.ToString();
					text.text = prettyXml;
/*
					var xmlReader = XmlReader.Create(new StringReader(text.text));
					var newText = "";
					while (xmlReader.Read()) {
						if (xmlReader.NodeType == XmlNodeType.Text) {
							var textContent = xmlReader.ReadContentAsString();
							newText += xmlReader.LocalName + ": " + textContent + "\n";
							//Console.WriteLine(xmlReader.LocalName + ": " + xmlReader.ReadContentAsString());
						}
					}
					text.text = newText;
*/
				}
				report = new CReport {texts = textsAndMetadata.texts, metadata = textsAndMetadata.metadata, fields = textsAndMetadata.fields};
			}
		}

		public CReport report;

		private int _reportCounter;

		public string ShortName {
			get {
				try {
                    //TODO:Pull out a good name for the report
					//if (report.metadata.fields != null && report.metadata.fields.dtg != null) return report.metadata.fields.dtg;
					return "Report " + ++_reportCounter;
				}
				catch (Exception e) {
					return "Report " + ++_reportCounter;
				}
			}
		}

		private ReportView ReportView { get; set; }

		public int TextsCount {
			get { return report.texts.Count; }
		}

		public Action<object> OpenReportCommand {
			get { return o => { ReportVisible = true; }; }
		}

		public bool ReportVisibleSetOnly {
			set {
				if (_reportVisible == value) return;
				_reportVisible = value;
				RaisePropertyChanged("ReportVisible");
			}
		}

		public bool ReportVisible {
			get { return _reportVisible; }
			set {
				if (value == _reportVisible) return;
				if (_reportVisible = value) {
					if (ReportView == null) ReportView = new ReportView {ReportViewModel = {GetReportResponse = this}};
					new AddRemoveDocumentViewMessage(null, null) {
						Operation = Operation.Add,
						ReportView = ReportView,
						TabText = ReportView.ReportViewModel.GetReportResponse.ShortName
					}.Send();
				}
				else
					new AddRemoveDocumentViewMessage(null, null) {
						Operation = Operation.Remove,
						ReportView = ReportView
					}.Send();
				RaisePropertyChanged("ReportVisible");
			}
		}
	}
}