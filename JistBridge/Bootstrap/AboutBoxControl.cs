using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI.AboutBox;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class AboutBoxControl : IBootstrapTask {
		internal AboutBoxControl() {
			ShowAboutBoxMessage.Register(this, msg => new ShowDialogMessage(msg.Sender, msg.Target) {
				ContainedControl = new AboutBox(),
			}.Send());
		}
	}
}