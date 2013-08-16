﻿using System;
using GalaSoft.MvvmLight;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using System.ComponentModel.Composition;
using JistBridge.Messages;

namespace JistBridge.UI.RichTextBox
{
	[Export(typeof(IRichTextBoxViewModel))]
	public class RichTextBoxViewModel : ViewModelBase, IRichTextBoxViewModel
	{
	    private Report _report;

	    private Chain _currentChain;

		public const string ReportContentsPropertyName = "ReportContents";

	    public string ReportContents
		{
			get { return _report.ReportText; }
		}

	    public Markup ReportMarkup
	    {
	        get { return _report.ReportMarkup; }
	    }

	    [ImportingConstructor]
		public RichTextBoxViewModel(IReportService reportService)
		{
            reportService.GetReport(
				(item, error) =>
				{
					if (error != null)
					{
						// Report error here
						return;
					}

					_report = item;
				});
		}

	}
}