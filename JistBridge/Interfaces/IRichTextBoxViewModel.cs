using JistBridge.Data.Model;

namespace JistBridge.Interfaces
{
	public interface IRichTextBoxViewModel
	{
		string ReportContents { get;}
        Markup ReportMarkup { get; }
	}
}