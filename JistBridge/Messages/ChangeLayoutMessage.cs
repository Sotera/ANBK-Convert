namespace JistBridge.Messages {
	public enum LayoutType {
		Peacock,
		Circle,
		Group
	}

	public class ChangeLayoutMessage : BaseMessage<ChangeLayoutMessage> {
		public LayoutType LayoutType { get; set; }

		public ChangeLayoutMessage(object sender, object target)
			: base(sender, target) {}
	}
}