using JistBridge.Data.ReST;

namespace JistBridge.Messages {
	public class ReportReceivedMessage : BaseMessage<ReportReceivedMessage> {
		internal GetReportResponse GetReportResponse { get; set; }
		public ReportReceivedMessage(object sender, object target)
			: base(sender, target) {}
	}
}