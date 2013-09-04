using System.ComponentModel.Composition;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class UserCredentialsControl : IBootstrapTask {
		public static ValidateUserResponse UserCredentials { get; set; }
		internal UserCredentialsControl() {
			UserCredentialsMessage.Register(this, msg => {
				UserCredentials = msg.UserCredentials;
				new SetMainWindowTitleMessage(this, null)
				{Title = "[User: " + UserCredentials.UserInfo.userName + "]"}.Send();
			});
		}
	}
}