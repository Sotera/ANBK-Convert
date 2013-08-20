using System.Collections.Generic;

namespace JistBridge.Data.ReST {
	public class GetReportResponse {
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
	}
}