namespace JistBridge.Messages {
	public class SetMainWindowTitleMessage : BaseMessage<SetMainWindowTitleMessage> {
		public string Title { get; set; }

		public SetMainWindowTitleMessage(object sender, object target)
			: base(sender, target) {}
	}
}