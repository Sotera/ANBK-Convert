using System;

namespace JistBridge.Data.Model
{
    public interface IReportService
    {
        void GetReport(Action<Report, Exception> callback);
    }
}