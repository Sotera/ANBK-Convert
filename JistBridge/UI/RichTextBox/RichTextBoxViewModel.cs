using GalaSoft.MvvmLight;
using JistBridge.Data.Model;

namespace JistBridge.UI.RichTextBox
{

    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class RichTextBoxViewModel : ViewModelBase
    {
        private IReportService _reportService;
        private Report _report;
        
        public const string ReportContentsPropertyName = "ReportContents";
        private string _reportContents = string.Empty;


        public string ReportContents
        {
            get
            {
                return _reportContents;
            }

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

        /// <summary>
        /// Initializes a new instance of the RichTextBoxViewModel class.
        /// </summary>
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