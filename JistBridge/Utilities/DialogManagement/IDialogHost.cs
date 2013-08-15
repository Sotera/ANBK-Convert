using System.Windows;

namespace JistBridge.Utilities.DialogManagement
{
	interface IDialogHost
	{
		void ShowDialog(DialogBaseControl dialog);
		void HideDialog(DialogBaseControl dialog);
		FrameworkElement GetCurrentContent();
	}
}