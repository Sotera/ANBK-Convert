using System;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Utilities.DialogManagement;
using JistBridge.Utilities.DialogManagement.Interfaces;

namespace JistBridge.Messages {
	public class ShowModalDialogMessage : BaseMessage<ShowModalDialogMessage> {
		private static IDialogManager _dialogManager;

		public enum DialogType {
			Message,
			Wait,
			Progress,
			Custom
		}

		public DialogType Type { get; set; }
		public Control ContainedControl { get; set; }
		public DialogMode DialogMode { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }

		public static void SetDefaultContentControl(ContentControl contentControl) {
			_dialogManager = new DialogManager(contentControl, DispatcherHelper.UIDispatcher);
		}

		public IDialogManager DialogManager {
			get {
				if (_dialogManager == null) {
					var msg = "Call ShowModalDialogMessage.SetDefaultContentControl(...)";
					msg += " with the MainWindow (StartupUri) of the application before";
					msg += " using the DialogManager.";
					throw new Exception(msg);
				}
				return _dialogManager;
			}
		}

		public ShowModalDialogMessage(object sender, object target)
			: base(sender, target) {
			Title = "";
			Text = "";
			Type = DialogType.Message;
		}
	}
}