using System;
using JistBridge.Data.Model;

namespace JistBridge.Interfaces {
	public interface IReportService {
		void GetReport(Action<Report, Exception> callback);
	}
}