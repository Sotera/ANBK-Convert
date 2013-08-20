using System.Collections.Generic;
using System.Windows.Documents;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using System;
using System.ComponentModel.Composition;

namespace JistBridge.Data.Model
{
	[Export(typeof(IReportService))]
	public class ReportService : IReportService
	{
		public void GetReport(Action<Report, Exception> callback)
		{
			// Use this to connect to the actual data service

			var report = new Report
			{
                ReportMarkup = new Markup(),
			    ReportText = "Tom was seen with Joe at the local 7-11",
                ReportResponse = new GetReportResponse
                {
                    report =  new GetReportResponse.Report
                    {
                        texts = new List<GetReportResponse.Report.Text>()
                    }
                 
                }
			};
		    var text1 = new GetReportResponse.Report.Text
		    {
		        offset = 1,
		        text = "Tom was seen with Joe at the local 7-11"
		    };
            var text2 = new GetReportResponse.Report.Text
		    {
		        offset = 1,
		        text = "Tom then left and was seen skulking around the walmart."
		    };

            report.ReportResponse.report.texts.Add(text1);
            report.ReportResponse.report.texts.Add(text2);
			callback(report, null);
		}
	}
}