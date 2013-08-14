using GalaSoft.MvvmLight;
using JistBridge.Data.Model;
using JistBridge.Interfaces;
using System.ComponentModel.Composition;

namespace JistBridge.UI.RichTextBox
{
	[Export(typeof(IRichTextBoxViewModel))]
	public class RichTextBoxViewModel : ViewModelBase, IRichTextBoxViewModel
	{
		private IReportService _reportService;
		private Report _report;

		public const string ReportContentsPropertyName = "ReportContents";
		private string _reportContents = string.Empty;

		public string ReportContents
		{
			get { return _reportContents; }

			set
			{
				if (_reportContents.Equals(value))
				{
					return;
				}

				_reportContents = value;
				RaisePropertyChanged(ReportContentsPropertyName);
			}
		}

		[ImportingConstructor]
		public RichTextBoxViewModel(IReportService reportService)
		{
			_reportService = reportService;
			_reportService.GetReport(
				(item, error) =>
				{
					if (error != null)
					{
						// Report error here
						return;
					}

					_report = item;
					ReportContents = _report.ReportText;
				});
		}
	}
}