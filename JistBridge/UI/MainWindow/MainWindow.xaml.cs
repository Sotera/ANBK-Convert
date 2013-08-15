using JistBridge.Interfaces;
using JistBridge.Messages;
using System.ComponentModel.Composition;

namespace JistBridge.UI.MainWindow
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private const int SplashScreenDelayMS = 3000;

		public MainWindow()
		{
			InitializeComponent();
			ShowModalDialogMessage.SetDefaultContentControl(this);
			new QueueMefComposeMessage(null, null, this).Send();
			new HideSplashScreenMessage(null, null).SendAfterWaiting(SplashScreenDelayMS);
		}

		[Import]
		public IMainWindowViewModel MainWindowViewModel
		{
			set { DataContext = value; }
		}
	}
}