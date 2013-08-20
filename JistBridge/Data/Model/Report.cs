using JistBridge.Data.ReST;

namespace JistBridge.Data.Model
{
	public class Report
	{
		public string ReportText { get; set; }
        public Markup ReportMarkup { get; set; }
        public GetReportResponse ReportResponse { get; set; } 
	}
}