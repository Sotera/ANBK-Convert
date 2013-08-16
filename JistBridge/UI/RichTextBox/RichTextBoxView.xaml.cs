using JistBridge.Interfaces;
using JistBridge.Messages;
using System.ComponentModel.Composition;

namespace JistBridge.UI.RichTextBox
{
	/// <summary>
	/// Interaction logic for RichTextBoxView.xaml
	/// </summary>
	public partial class RichTextBoxView
	{
		[Import]
		public IRichTextBoxViewModel RichTextBoxViewModel
		{
			set
			{
				DataContext = value;
			}
		    get
		    {
		        if (!(DataContext is RichTextBoxViewModel))
		            return null;
		        return DataContext as RichTextBoxViewModel;
		    }
		}

		public RichTextBoxView()
		{
			InitializeComponent();
			new QueueMefComposeMessage(null, null, this, msg =>
			{
			    new RichTextBoxLoadedMessage(msg.Sender, msg.Target).SendAfterWaiting(1000);
			}).Send();
        }
	}
}