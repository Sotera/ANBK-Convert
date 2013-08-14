using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.RichTextBox {
	/// <summary>
	/// Interaction logic for RichTextBoxView.xaml
	/// </summary>
	public partial class RichTextBoxView {
		[Import]
		public IRichTextBoxViewModel RichTextBoxViewModel {
			set {
				DataContext = value;
			}
		}

		public RichTextBoxView() {
			InitializeComponent();
			new QueueMefComposeMessage(null, null, this).Send();
		}
	}
}