using JistBridge.Data.Model;
using JistBridge.Data.ReST;

namespace JistBridge.Messages {
	public class UserCredentialsMessage : BaseMessage<UserCredentialsMessage> {
		public ValidateUserResponse UserCredentials { get; set; }

		public UserCredentialsMessage(object sender, object target)
			: base(sender, target) {
		}
	}
}