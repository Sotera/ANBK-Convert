using JistBridge.Data.ReST;

namespace JistBridge.Messages
{
    public class SaveReportRestMessage : BaseMessage<SaveReportRestMessage>
    {
        public GetReportResponse ReportData { get; set; }
        public SaveReportRestMessage(object sender, object target)
            : base(sender, target)
        {
           
        }


    }
}
