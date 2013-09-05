using System;
using System.Collections.Generic;
using JistBridge.Data.Model;
using JistBridge.Data.ReST;

namespace JistBridge.Messages.ANBK {
	public class SendChainInfoToANBKMessage : BaseMessage<SendChainInfoToANBKMessage> {
		public enum ActionType {
			CreateNewChain,
			ModifyLeftNodeText,
			ModifyLinkText,
			ModifyRightNodeText,
            ToggleRightNodeSelection,
            ToggleLeftNodeSelection,
            ToggleLinkSelection
		};
		public ActionType Action { get; set; }
		public Chain Chain { get; set; }
	    public Dictionary<string,string> Fields { get; set; }
	    public GetReportResponse.CReport.CMetadata MetaData { get; set; }

	    public SendChainInfoToANBKMessage(object sender, object target, Action<SendChainInfoToANBKMessage> cb)
			: base(sender, target, cb) {}
	}
}