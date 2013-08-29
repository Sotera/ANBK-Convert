using System.ComponentModel.Composition;
using JistBridge.Data.ReST;

namespace JistBridge.Data.Model
{
    [Export(typeof(ReportData))]
    public class ReportData
    {
        [Import(typeof(Markup))]
        public Markup ReportMarkup { get; set; }
        public GetReportResponse ReportResponse { get; set; }
        public ReportData()
        { }


    }
}