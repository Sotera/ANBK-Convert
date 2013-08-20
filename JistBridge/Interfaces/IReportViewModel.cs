using System.Windows.Documents;
using JistBridge.Data.Model;

namespace JistBridge.Interfaces
{
    public interface IReportViewModel
    {
        FlowDocument ReportDocument { get; }
        Markup ReportMarkup { get; }
    }
}
