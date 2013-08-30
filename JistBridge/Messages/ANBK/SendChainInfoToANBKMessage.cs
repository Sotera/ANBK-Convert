using System;

namespace JistBridge.Messages.ANBK {
	public class SendChainInfoToANBKMessage : BaseMessage<SendChainInfoToANBKMessage> {
		public enum ActionType {
			CreateNewChain,
			ModifyLeftNodeText,
			ModifyLinkText,
			ModifyRightNodeText
		};
		public ActionType Action { get; set; }
		public string Text { get; set; }
		public int LeftNodeId;
		public int LinkId;
		public int RightNodeId;
		public SendChainInfoToANBKMessage(object sender, object target, Action<SendChainInfoToANBKMessage> cb)
			: base(sender, target, cb) {}
	}
}