namespace JistBridge.Messages {
	public enum FitType {
		ActualSize,
		Height,
		Window,
		SelectionInWindow
	}

	public class FitChartMessage : BaseMessage<FitChartMessage> {
		public FitType FitType { get; set; }

		public FitChartMessage(object sender, object target)
			: base(sender, target) {}
	}
}