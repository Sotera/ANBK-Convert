namespace JistBridge.Messages {
	public class ShowOptionsDialogMessage : BaseMessage<ShowOptionsDialogMessage> {
		public object PropertiesObject { get; set; }

		public ShowOptionsDialogMessage(object sender, object target)
			: base(sender, target) {}
	}
}