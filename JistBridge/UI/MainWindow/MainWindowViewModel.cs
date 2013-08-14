using System.ComponentModel.Composition;
using GalaSoft.MvvmLight;
using JistBridge.Interfaces;

namespace JistBridge.UI.MainWindow {
	[Export(typeof (IMainWindowViewModel))]
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel {
		public static IAppConfiguration StaticAppConfiguration { get; private set; }

		[Import(typeof (IAppConfiguration))]
		public IAppConfiguration AppConfiguration {
			get { return StaticAppConfiguration; }
			set { StaticAppConfiguration = value; }
		}

		public string Title {
			get {
				return AppConfiguration.ApplicationName;
			}
		}
	}
}