using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.UI.MainWindow {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow {
		private const int SplashScreenDelayMS = 3000;

		public MainWindow() {
			InitializeComponent();
			new QueueMefComposeMessage(null, null, this).SendAfterWaiting(SplashScreenDelayMS);
		}

		[Import]
		public IMainWindowViewModel MainWindowViewModel {
			set { DataContext = value; }
		}
	}
}