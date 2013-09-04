using System.Windows.Documents;
using JistBridge.Data.Model;
using JistBridge.Data.ReST;
using JistBridge.Messages.ANBK;

namespace JistBridge.Interfaces {
	public interface IReportViewModel {
		FlowDocument ReportDocument { get; }
		Markup ReportMarkup { get; }
		GetReportResponse GetReportResponse { get; set; }
	}
}