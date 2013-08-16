using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JistBridge.Data.Model;

namespace JistBridge.Messages
{
    public class ChainAddedMessage: BaseMessage<ChainAddedMessage>
    {
        public Chain Chain;

        public Guid MarkupId { get; private set; }

        public ChainAddedMessage(object sender, object target, Guid markupId, Chain chain)
			: base(sender, target)
        {
            MarkupId = markupId;
            Chain = chain;
        }

        
    }
}
