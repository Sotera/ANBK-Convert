using JistBridge.UI.ReportView;

namespace JistBridge.Messages {
	public enum Operation {
		Add,
		Remove
	}

	public class AddRemoveReportViewMessage : BaseMessage<AddRemoveReportViewMessage> {
		public Operation Operation { get; set; }
		public ReportView ReportView { get; set; }
		public string TabText { get; set; }

		public AddRemoveReportViewMessage(object sender, object target)
			: base(sender, target) {}
	}
}