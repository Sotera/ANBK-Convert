using System;

namespace JistBridge.Messages
{
	public class QueueMefComposeMessage : BaseMessage<QueueMefComposeMessage>
	{
		public object MefTarget { get; set; }

        public QueueMefComposeMessage(object sender, object target, object mefTarget, Action<QueueMefComposeMessage> callback)
			: base(sender, target, callback)
		{
			MefTarget = mefTarget;
		}
	}
}