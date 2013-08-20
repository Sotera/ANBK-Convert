using System.Windows;
using JistBridge.Messages;

namespace JistBridge.UI.RichTextBox {
	/// <summary>
	/// Interaction logic for RichTextBoxView.xaml
	/// </summary>
	public partial class RichTextBoxView {
		private void RtbLoaded(object obj, DependencyPropertyChangedEventArgs args) {
			new RichTextBoxLoadedMessage(this, null).Send();
			IsVisibleChanged -= RtbLoaded;
		}

		public RichTextBoxView() {
			InitializeComponent();
			IsVisibleChanged += RtbLoaded;
			new QueueMefComposeMessage(null, null, this, null).Send();
		}
	}
}