using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class AboutBoxControl : IBootstrapTask {
		internal AboutBoxControl() {
			ShowAboutBoxMessage.Register(this, msg => {
				var showModalDialogMessage = new ShowModalDialogMessage(msg.Sender, msg.Target);
				showModalDialogMessage.Send();
			});
		}
	}
}