using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.SplashScreen;
using JistBridge.UI.AboutBox;
using JistBridge.Utilities.DialogManagement;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class AboutBoxControl : IBootstrapTask {
		internal AboutBoxControl() {
			ShowAboutBoxMessage.Register(this, msg => new ShowModalDialogMessage(msg.Sender, msg.Target) {
				Type = ShowModalDialogMessage.DialogType.Custom,
				ContainedControl = new AboutBox(),
				DialogMode = DialogMode.Ok
			}.Send());
		}
	}
}