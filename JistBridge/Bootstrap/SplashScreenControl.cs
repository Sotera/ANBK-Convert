using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.SplashScreen;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class SplashScreenControl : IBootstrapTask {
		internal SplashScreenControl() {
			Splasher.ShowSplash(new SplashScreenWindow());

			HideSplashScreenMessage.Register(this, msg => {
				HideSplashScreenMessage.Unregister(this);
				Splasher.CloseSplash();
			});
		}
	}
}