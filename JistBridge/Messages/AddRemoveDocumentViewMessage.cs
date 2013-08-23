using System;
using System.Windows.Controls;

namespace JistBridge.Messages {
	public enum Operation {
		Add,
		Remove
	}

	public class AddRemoveDocumentViewMessage : BaseMessage<AddRemoveDocumentViewMessage> {
		public Operation Operation { get; set; }
		public Control ReportView { get; set; }
		public string TabText { get; set; }

		public AddRemoveDocumentViewMessage(object sender, object target)
			: base(sender, target) {}

		public AddRemoveDocumentViewMessage(object sender, object target, Action<AddRemoveDocumentViewMessage> callback)
			: base(sender, target, callback) {}
	}
}