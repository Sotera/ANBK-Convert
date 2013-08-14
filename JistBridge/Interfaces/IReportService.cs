using JistBridge.Data.Model;
using System;

namespace JistBridge.Interfaces
{
	public interface IReportService
	{
		void GetReport(Action<Report, Exception> callback);
	}
}