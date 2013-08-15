using System.Windows.Controls;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Utilities.DialogManagement;
using JistBridge.Utilities.DialogManagement.Interfaces;

namespace JistBridge.Messages {
	public class ShowModalDialogMessage : BaseMessage<ShowModalDialogMessage> {
		private static IDialogManager _dialogManager;

		public static void SetDefaultContentControl(ContentControl contentControl) {
			_dialogManager = new DialogManager(contentControl, DispatcherHelper.UIDispatcher);
		}

		public IDialogManager DialogManager {
			get { return _dialogManager; }
		}

		public ShowModalDialogMessage(object sender, object target)
			: base(sender, target) {}
	}
}