using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using JistBridge.Interfaces;
using JistBridge.Messages;
using System.ComponentModel.Composition;

namespace JistBridge.UI.MainWindow
{
	[Export(typeof(IMainWindowViewModel))]
	public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
	{
		public static IAppConfiguration StaticAppConfiguration { get; private set; }

		[Import(typeof(IAppConfiguration))]
		public IAppConfiguration AppConfiguration
		{
			get { return StaticAppConfiguration; }
			set { StaticAppConfiguration = value; }
		}

		public string StatusMessage
		{
			get { return "Ready"; }
		}

		public RelayCommand ShowAboutBoxCommand
		{
			get { return new RelayCommand(() => new ShowAboutBoxMessage(null, null).Send()); }
		}

		public RelayCommand ExitCommand
		{
			get { return new RelayCommand(() => new ShutdownApplicationMessage(null, null).Send()); }
		}

		public string Title
		{
			get { return AppConfiguration.ApplicationName; }
		}
	}
}