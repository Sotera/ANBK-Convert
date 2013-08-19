using System;
using JistBridge.Data.Model;

namespace JistBridge.Messages
{
    public enum ChainStatus
    {
        Finished,
        LeftFragmentCanceled,
        CenterFragmentCanceled,
        RightFragmentCanceled,
        LeftFragmentAdded,
        CenterFragmentAdded,
        RightFragmentAdded
    }

    public class ChainStatusMessage : BaseMessage<ChainStatusMessage>
    {
        public Chain Chain { get; private set; }
        public ChainStatus Status { get; private set; }
        public Markup Markup { get; private set; }

        public ChainStatusMessage(object sender, object target, Markup markup, Chain chain, ChainStatus status)
            : base(sender, target)
        {
            Markup = markup;
            Chain = chain;
            Status = status;
        }


    }
}
