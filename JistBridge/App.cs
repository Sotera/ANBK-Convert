using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Bootstrap;
using JistBridge.Messages;
using JistBridge.Utilities.PInvoke;
using NLog;

// ReSharper disable ObjectCreationAsStatement

namespace JistBridge {
	public partial class App : ISingleInstanceApp {
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();
		private const string MyGuid = "{AA1136B6-A1F3-4DCA-99D1-8051E2FCB423}";

		[STAThread]
		public static void Main() {
			//Because we may be interacting with the external COM server Analyst's Notebook, which itself
			//is designed to run only one user instance at a time, it will help with some lifetime headaches
			//if we operate the same way. So, if someone trys to start us up and we're already running we
			//will bring our already instance forward on the console rather than starting another instance.
			if (SingleInstance<App>.InitializeAsFirstInstance(MyGuid)) {
				Thread.CurrentThread.CurrentCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
				Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern = "dd HHmmZ MMM yyyy";

				//Initialize a nice UI thread dispatcher for our use
				DispatcherHelper.Initialize();

				new Bootstrapper(() => { new App(); });
				SingleInstance<App>.Cleanup();
			}
			else
				NativeMethods.PostMessage(
					(IntPtr) NativeMethods.HWND_BROADCAST,
					NativeMethods.WM_SHOWME,
					IntPtr.Zero,
					IntPtr.Zero);
		}

		public App() {
			StartupUri = new Uri("UI/MainWindow/MainWindow.xaml", UriKind.Relative);
			ShutdownApplicationMessage.Register(this, msg => Current.Shutdown());
			Run();
		}

		public bool SignalExternalCommandLineArgs(IList<string> args) {
			//Handle command line args reconfigure running instance

			return true;
		}
	}
}