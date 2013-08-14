﻿using System;
using System.ComponentModel.Composition;
using JistBridge.Interfaces;

namespace JistBridge.Data.Model {
	[Export(typeof (IReportService))]
	public class ReportService : IReportService {
		public void GetReport(Action<Report, Exception> callback) {
			// Use this to connect to the actual data service

			var report = new Report {ReportText = "Tom was seen with Joe at the local 7-11"};
			callback(report, null);
		}
	}
}