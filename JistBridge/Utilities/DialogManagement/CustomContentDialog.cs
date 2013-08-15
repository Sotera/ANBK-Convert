using System.Windows.Threading;
using JistBridge.Utilities.DialogManagement.Interfaces;

namespace JistBridge.Utilities.DialogManagement
{
	class CustomContentDialog : DialogBase, ICustomContentDialog
	{
		public CustomContentDialog(
			IDialogHost dialogHost, 
			DialogMode dialogMode,
			object content,
			Dispatcher dispatcher)
			: base(dialogHost, dialogMode, dispatcher)
		{
			SetContent(content);
		}
	}
}