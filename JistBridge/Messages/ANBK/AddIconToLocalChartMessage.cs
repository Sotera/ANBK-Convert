namespace JistBridge.Messages.ANBK {
	public class AddIconToLocalChartMessage : BaseMessage<AddIconToLocalChartMessage> {
		public string Label { get; set; }
		public AddIconToLocalChartMessage(object sender, object target)
			: base(sender, target) {}
	}
}