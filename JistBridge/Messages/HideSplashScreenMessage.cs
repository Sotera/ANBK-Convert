namespace JistBridge.Messages
{
	public class HideSplashScreenMessage : BaseMessage<HideSplashScreenMessage>
	{
		public object MefTarget { get; set; }

		public HideSplashScreenMessage(object sender, object target)
			: base(sender, target)
		{
		}
	}
}