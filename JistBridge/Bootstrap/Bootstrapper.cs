using System;
using JistBridge.Messages;
using JistBridge.SplashScreen;
using JistBridge.Utilities.DialogManagement;
using JistBridge.Utilities.MefHelpers;

namespace JistBridge.Bootstrap {
	internal class Bootstrapper {
		public Bootstrapper(Action callback) {
			ExecutingAssemblyMefHelper.DoMefCompose(this);
			QueueMefComposeMessage.Register(this, msg => ExecutingAssemblyMefHelper.DoMefCompose(msg.MefTarget));

			ShowModalDialogMessage.Register(this,
				msg => { msg.DialogManager.CreateMessageDialog("Test", "I'm a dialog", DialogMode.Ok).Show(); });

			ShowAboutBoxMessage.Register(this, msg => {
				var showModalDialogMessage = new ShowModalDialogMessage(msg.Sender, msg.Target);
				showModalDialogMessage.Send();
			});

			HideSplashScreenMessage.Register(this, msg => {
				HideSplashScreenMessage.Unregister(this);
				Splasher.CloseSplash();
			});


			if (callback != null) {
				callback();
			}
		}
	}
}