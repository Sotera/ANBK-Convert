using JistBridge.Data.Model;

namespace JistBridge.Messages
{
    public class ReportModifiedMessage : BaseMessage<ReportModifiedMessage>
    {
        public Markup ReportMarkup { get; set; }

        public ReportModifiedMessage(object sender, object target, Markup markup)
            : base(sender, target)
        {
            ReportMarkup = markup;
        }
    }

}
