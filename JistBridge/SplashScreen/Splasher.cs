using System.Windows;

namespace JistBridge.SplashScreen {
	public static class Splasher {
		private static Window _splashScreen;

		public static void ShowSplash(Window splashScreen) {
			(_splashScreen = splashScreen).Show();
		}

		public static void CloseSplash() {
			if (_splashScreen != null) {
				_splashScreen.Close();
			}
		}
	}
}