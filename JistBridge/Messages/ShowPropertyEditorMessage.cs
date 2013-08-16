namespace JistBridge.Messages {
	public class ShowPropertyEditorMessage : BaseMessage<ShowPropertyEditorMessage> {
		public object PropertiesObject { get; set; }

		public ShowPropertyEditorMessage(object sender, object target)
			: base(sender, target) {}
	}
}