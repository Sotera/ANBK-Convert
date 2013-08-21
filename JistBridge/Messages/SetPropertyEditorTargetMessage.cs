namespace JistBridge.Messages {
	public class SetPropertyEditorTargetMessage : BaseMessage<SetPropertyEditorTargetMessage> {
		public object PropertiesObject { get; set; }

		public SetPropertyEditorTargetMessage(object sender, object target)
			: base(sender, target) {}
	}
}