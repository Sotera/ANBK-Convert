using System;
using JistBridge.Data.Model;

namespace JistBridge.Messages
{
    public enum ChainStatus
    {
        Finished,
        FragmentCanceled,
        LeftFragmentAdded,
        CenterFragmentAdded,
        RightFragmentAdded
    }

    public class ChainStatusMessage : BaseMessage<ChainStatusMessage>
    {
        public Chain Chain { get; private set; }
        public ChainStatus Status { get; private set; }
        public Guid MarkupId { get; private set; }

        public ChainStatusMessage(object sender, object target, Guid markupId, Chain chain, ChainStatus status)
            : base(sender, target)
        {
            MarkupId = markupId;
            Chain = chain;
            Status = status;
        }


    }
}
