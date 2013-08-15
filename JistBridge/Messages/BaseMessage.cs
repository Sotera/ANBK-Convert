using System;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace JistBridge.Messages {
	public class BaseMessage<T> : NotificationMessageAction<T> where T : class {
		public BaseMessage(object sender, object target, Action<T> callback)
			: base(sender, target, "", callback ?? (msg => { })) {}

		public BaseMessage(object sender, object target)
			: base(sender, target, "", msg => { }) {}

		public static void Register(object recipient, Action<T> action) {
			Messenger.Default.Register(recipient, action);
		}

		public static void Unregister(object recipient) {
			Messenger.Default.Unregister<T>(recipient);
		}

		public void SendAfterWaiting(int milleseconds) {
			InternalSend(milleseconds);
		}

		public void Send() {
			if (ViewModelBase.IsInDesignModeStatic) {
				return;
			}
			InternalSend();
		}

		private void InternalSend(int delayInMilleseconds = 0) {
			if (delayInMilleseconds > 0) {
				new Thread(() => {
					Thread.Sleep(delayInMilleseconds);
					InternalSendOnUIThread();
				}) {IsBackground = true}.Start();
			}
			else {
				InternalSendOnUIThread();
			}
		}

		private void InternalSendOnUIThread() {
			if (!DispatcherHelper.UIDispatcher.CheckAccess()) {
				DispatcherHelper.UIDispatcher.Invoke(() => Messenger.Default.Send(this as T));
			}
			else {
				Messenger.Default.Send(this as T);
			}
		}
	}

	internal class BaseMessage {}
}