using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JistBridge.Data.Model;

namespace JistBridge.Messages
{
    class FragmentSelectedMessage : BaseMessage<FragmentSelectedMessage>
    {
        public Fragment Fragment;

        public Guid MarkupId { get; private set; }
        
        public FragmentSelectedMessage(object sender, object target,Guid markupId, Fragment fragment)
			: base(sender, target)
        {
            MarkupId = markupId;
            Fragment = fragment;
        }

        
    }
}
