using JistBridge.Data.ReST;

namespace JistBridge.Data.Model
{
	public class Report
	{
		private Markup _reportMarkup;

		public Markup ReportMarkup {
			get { return _reportMarkup ?? (_reportMarkup = new Markup()); }
		}

		public GetReportResponse ReportResponse { get; set; } 
	}
}