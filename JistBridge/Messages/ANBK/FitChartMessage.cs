namespace JistBridge.Messages {
	public enum PointerType {
		Pointer,
		Drag
	}

	public class ChangePointerModeMessage : BaseMessage<ChangePointerModeMessage> {
		public PointerType FitType { get; set; }

		public ChangePointerModeMessage(object sender, object target)
			: base(sender, target) {}
	}
}