namespace JistBridge.Messages
{
	public class ShutdownApplicationMessage : BaseMessage<ShutdownApplicationMessage>
	{
		public ShutdownApplicationMessage(object sender, object target)
			: base(sender, target)
		{
		}
	}
}