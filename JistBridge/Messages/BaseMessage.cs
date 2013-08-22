﻿using System;
using System.ComponentModel;
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

		public static void Register(object recipient, object token, Action<T> action) {
			Messenger.Default.Register(recipient, token, action);
		}

		public static void Register(object recipient, Action<T> action) {
			Messenger.Default.Register(recipient, action);
		}

		public static void Unregister(object recipient) {
			Messenger.Default.Unregister<T>(recipient);
		}

		public void SendAfterWaiting(int milleseconds) {
			InternalSend(milleseconds);
		}

		public void Send(object token = null) {
			InternalSend(0, token);
		}

		private void InternalSend(int delayInMilleseconds = 0, object token = null) {
			if (ViewModelBase.IsInDesignModeStatic) {
				return;
			}
			if (delayInMilleseconds > 0) {
				var worker = new BackgroundWorker();
				worker.DoWork += (o, ea) => {
					Thread.Sleep(delayInMilleseconds);
					InternalSendOnUIThread(token);
				};
				worker.RunWorkerAsync();
/*
				new Thread(() => {
					Thread.Sleep(delayInMilleseconds);
					InternalSendOnUIThread();
				}) {IsBackground = true}.Start();
*/
			}
			else {
				InternalSendOnUIThread(token);
			}
		}

		private void InternalSendOnUIThread(object token = null) {
			if (!DispatcherHelper.UIDispatcher.CheckAccess()) {
				DispatcherHelper.UIDispatcher.Invoke(() => {
					if (token != null) {
						Messenger.Default.Send(this as T, token);
					}
					else {
						Messenger.Default.Send(this as T);
					}
				});
			}
			else {
				if (token != null) {
					Messenger.Default.Send(this as T, token);
				}
				else {
					Messenger.Default.Send(this as T);
				}
			}
		}
	}

	internal class BaseMessage {}
}