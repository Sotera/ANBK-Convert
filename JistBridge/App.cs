using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Messages;
using JistBridge.SplashScreen;
using JistBridge.Utilities.DialogManagement;
using NLog;

// ReSharper disable ObjectCreationAsStatement

namespace JistBridge {
	public partial class App {
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		[STAThread]
		public static void Main() {
			_log.Fatal("Test the logger!");

			Thread.CurrentThread.CurrentCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
			Thread.CurrentThread.CurrentCulture.DateTimeFormat.LongDatePattern = "dd HHmmZ MMM yyyy";

			//Initialize a nice UI thread dispatcher for our use
			DispatcherHelper.Initialize();

			Splasher.ShowSplash(new SplashScreenWindow());

			new Bootstrapper();
		}

		public App() {
			QueueMefComposeMessage.Register(this, msg => DoMefCompose(msg.MefTarget));
			ShutdownApplicationMessage.Register(this, msg => Current.Shutdown());

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

			StartupUri = new Uri("UI/MainWindow/MainWindow.xaml", UriKind.Relative);
			Run();
		}

		private class Bootstrapper {
			public Bootstrapper() {
				DoMefCompose(this);
				new App();
			}
		}

		private static AggregateCatalog _staticAggregateCatalog;

		private static AggregateCatalog AggregateCatalog {
			get {
				if (_staticAggregateCatalog != null) {
					return _staticAggregateCatalog;
				}
				_staticAggregateCatalog = new AggregateCatalog();
				_staticAggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
				return _staticAggregateCatalog;
			}
		}

		private static void DoMefCompose(object target) {
			try {
				DispatcherHelper.UIDispatcher.Invoke(
					() => {
						try {
							new CompositionContainer(AggregateCatalog).ComposeParts(target);
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