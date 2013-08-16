using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.MainWindow {
	public partial class MainWindow {
		public MainWindow() {
			InitializeComponent();
			ShowModalDialogMessage.SetDefaultContentControl(this);
			new QueueMefComposeMessage(null, null, this, null).Send();
		}

		[Import]
		public IMainWindowViewModel MainWindowViewModel {
			set { DataContext = value; }
		}
	}
}