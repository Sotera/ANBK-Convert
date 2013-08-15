using System;

namespace JistBridge.Messages {
	public class MessageCallbackTestMessage : BaseMessage<MessageCallbackTestMessage> {
		public MessageCallbackTestMessage(object sender, object target, Action<MessageCallbackTestMessage> callback)
			: base(sender, target, callback) {}

		public string TestString { get; set; }
	}
}