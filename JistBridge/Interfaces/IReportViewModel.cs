using JistBridge.Data.Model;

namespace JistBridge.Interfaces
{
    public interface IReportViewModel
    {
        string ReportContents { get; }
        Markup ReportMarkup { get; }
    }
}
