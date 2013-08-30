using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media.Imaging;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Properties;
using Xceed.Wpf.Toolkit;
using WindowStartupLocation = System.Windows.WindowStartupLocation;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class DialogControl : IBootstrapTask {
		internal DialogControl() {
			ShowDialogMessage.Register(this, ShowDialogBox);
		}

		private static void ShowDialogBox(ShowDialogMessage msg) {
/*
			var childWindow = new ChildWindow {
				Caption = msg.Title,
				IsModal = msg.IsModal,
				Content = msg.ContainedControl,
				WindowStartupLocation = WindowStartupLocation.Center,
				WindowState = WindowState.Open
			};
			msg.WindowContainer.Children.Add(childWindow);
*/
			var childWindow = new Window();
			childWindow.Icon = new BitmapImage(new Uri("pack://application:,,,/JistBridge;component/Resources/bridge.ico"));
			childWindow.Content = msg.ContainedControl;
			childWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			childWindow.Title = msg.Title;
			childWindow.SizeToContent = SizeToContent.WidthAndHeight;
			childWindow.ShowDialog();
		}
	}
}