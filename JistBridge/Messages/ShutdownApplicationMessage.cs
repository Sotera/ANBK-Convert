namespace JistBridge.Messages
{
	public class ShutdownApplicationMessage : BaseMessage<ShutdownApplicationMessage>
	{
		public object MefTarget { get; set; }

		public ShutdownApplicationMessage(object sender, object target)
			: base(sender, target)
		{
		}
	}
}