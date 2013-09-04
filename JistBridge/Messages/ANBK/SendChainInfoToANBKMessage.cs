using System;
using Interop.i2NotebookData;
using JistBridge.Data.Model;

namespace JistBridge.Messages.ANBK {
	public class SendChainInfoToANBKMessage : BaseMessage<SendChainInfoToANBKMessage> {
		public enum ActionType {
			CreateNewChain,
			ModifyLeftNodeText,
			ModifyLinkText,
			ModifyRightNodeText
		};
		public ActionType Action { get; set; }
		public Chain Chain { get; set; }
	    public SendChainInfoToANBKMessage(object sender, object target, Action<SendChainInfoToANBKMessage> cb)
			: base(sender, target, cb) {}
	}
}