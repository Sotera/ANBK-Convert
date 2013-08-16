using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JistBridge.Messages
{
    public class RichTextBoxLoadedMessage : BaseMessage<RichTextBoxLoadedMessage>
    {
        public RichTextBoxLoadedMessage(object sender, object target)
            : base(sender, target)
        {
            
        }
    }
}
