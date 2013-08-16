using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class ModalDialogControl : IBootstrapTask {
		internal ModalDialogControl() {
			ShowModalDialogMessage.Register(this, ShowDialogBox);
		}

		private static void ShowDialogBox(ShowModalDialogMessage msg) {
			switch (msg.Type) {
				case (ShowModalDialogMessage.DialogType.Message):
					break;
				case (ShowModalDialogMessage.DialogType.Custom):
					msg.DialogManager.CreateCustomContentDialog(msg.ContainedControl, msg.Title, msg.DialogMode).Show();
					break;
				case (ShowModalDialogMessage.DialogType.Wait):
					break;
				case (ShowModalDialogMessage.DialogType.Progress):
					break;
			}
		}
	}
}