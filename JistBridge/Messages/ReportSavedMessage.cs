using JistBridge.Data.ReST;

namespace JistBridge.Messages
{
    public class ReportSavedMessage : BaseMessage<ReportSavedMessage>
    {
        internal SaveReportResponse SaveReportResponse { get; set; }
        public ReportSavedMessage(object sender, object target)
            : base(sender, target) { }
    }

}
