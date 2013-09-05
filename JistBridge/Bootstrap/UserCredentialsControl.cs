using System;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class UserCredentialsControl : IBootstrapTask {
		private static ValidateUserResponse _userCredentials;

		public static ValidateUserResponse UserCredentials {
			get {
				if (_userCredentials == null) {
					new ShowDialogMessage(null, null) {
						Title = "User not logged in",
						IsModal = true,
						ContainedControl = new Label {Content = "User not logged in, please log in.", Width = 200, Height = 40},
					}.Send();
					throw new Exception("User not logged in.");
				}
				return _userCredentials;
			}
			set { _userCredentials = value; }
		}

		internal UserCredentialsControl() {
			UserCredentialsMessage.Register(this, msg => {
				UserCredentials = msg.UserCredentials;
				new SetMainWindowTitleMessage(this, null)
				{Title = "[User: " + UserCredentials.UserInfo.userName + "]"}.Send();
			});
		}
	}
}