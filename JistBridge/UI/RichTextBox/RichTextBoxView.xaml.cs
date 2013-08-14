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
		}

		public RichTextBoxView()
		{
			InitializeComponent();
			new QueueMefComposeMessage(null, null, this).Send();
		}
	}
}