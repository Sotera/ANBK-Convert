using System;
using System.Globalization;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Bootstrap;
using JistBridge.Messages;
using JistBridge.SplashScreen;
using JistBridge.Utilities.DialogManagement;
using JistBridge.Utilities.MefHelpers;
using NLog;

// ReSharper disable ObjectCreationAsStatement

namespace JistBridge {
	public partial class App {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		[STAThread]
		public static void Main() {
			Thread.CurrentThread.CurrentCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern = "dd HHmmZ MMM yyyy";

			//Initialize a nice UI thread dispatcher for our use
			DispatcherHelper.Initialize();

			Splasher.ShowSplash(new SplashScreenWindow());

			new Bootstrapper(() => {
				new App();
			});
		}

		public App() {
			StartupUri = new Uri("UI/MainWindow/MainWindow.xaml", UriKind.Relative);
			ShutdownApplicationMessage.Register(this, msg => Current.Shutdown());
			Run();
		}
	}
}