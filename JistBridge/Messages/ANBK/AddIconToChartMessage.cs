namespace JistBridge.Messages.ANBK {
	public class AddIconToChartMessage : BaseMessage<AddIconToChartMessage> {
		public string Label { get; set; }
		public AddIconToChartMessage(object sender, object target)
			: base(sender, target) {}
	}
}