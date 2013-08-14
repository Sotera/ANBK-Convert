using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.SplashScreen;

// ReSharper disable ObjectCreationAsStatement

namespace JistBridge {
	public partial class App {
		[STAThread]
		public static void Main() {
			Thread.CurrentThread.CurrentCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern = "dd HHmmZ MMM yyyy";

			//Initialize a nice UI thread dispatcher for our use
			DispatcherHelper.Initialize();

			Splasher.ShowSplash(new SplashScreenWindow());

			new Bootstrapper();
		}

		public App() {
			QueueMefComposeMessage.Register(this,
				msg => {
					QueueMefComposeMessage.Unregister(this);
					DoMefCompose(msg.MefTarget);
					Splasher.CloseSplash();
				});
			StartupUri = new Uri("UI/MainWindow/MainWindow.xaml", UriKind.Relative);
			Run();
		}

		private class Bootstrapper {
			public Bootstrapper() {
				DoMefCompose(this);
				new App();
			}
		}

		private static void DoMefCompose(object target) {
			try {
				DispatcherHelper.UIDispatcher.Invoke(
					() => {
						try {
							var catalog = new AggregateCatalog();
							catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
							var container = new CompositionContainer(catalog);
							container.ComposeParts(target);
						}
						catch (Exception e) {
							Console.WriteLine(e);
						}
					});
			}
			catch (Exception ex) {
				Current.MainWindow.Title = "MEF Composition FAILED";
				Trace.TraceError("MEF Composition FAILED: " + ex.Message);
			}
		}
	}
}