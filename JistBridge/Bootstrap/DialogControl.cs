using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using Xceed.Wpf.Toolkit;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class DialogControl : IBootstrapTask {
		internal DialogControl() {
			ShowDialogMessage.Register(this, ShowDialogBox);
		}

		private static void ShowDialogBox(ShowDialogMessage msg) {
			var childWindow = new ChildWindow {
				Caption = msg.Title,
				IsModal = msg.IsModal,
				Content = msg.ContainedControl,
				WindowStartupLocation = WindowStartupLocation.Center,
				WindowState = WindowState.Open
			};
			msg.WindowContainer.Children.Add(childWindow);
		}
	}
}