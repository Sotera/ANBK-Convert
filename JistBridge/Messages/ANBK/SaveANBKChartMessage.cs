using System;

namespace JistBridge.Messages.ANBK
{
    public class SaveANBKChartMessage : BaseMessage<SaveANBKChartMessage>
    {
        public SaveANBKChartMessage(object sender, object target, Action<SaveANBKChartMessage> cb)
            : base(sender, target, cb) { }
    }
    
}
