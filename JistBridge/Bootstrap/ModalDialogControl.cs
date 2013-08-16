using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Utilities.DialogManagement;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class ModalDialogControl : IBootstrapTask {
		internal ModalDialogControl() {
			ShowModalDialogMessage.Register(this,
				msg => { msg.DialogManager.CreateMessageDialog("Test", "I'm a dialog", DialogMode.Ok).Show(); });
		}
	}
}