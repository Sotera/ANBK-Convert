using System.Windows.Controls;
using NLog.Targets.Wrappers;

namespace JistBridge.Messages {
	public class ShowNLogWindowMessage : BaseMessage<ShowNLogWindowMessage> {
		public static AsyncTargetWrapper AsyncTargetWrapper { get; set; }
		public static RichTextBox RichTextBox { get; set; }

		public static void SetNLogWindowInfo(AsyncTargetWrapper asyncTargetWrapper,
			RichTextBox richTextBox) {
			AsyncTargetWrapper = asyncTargetWrapper;
			RichTextBox = richTextBox;
		}

		public ShowNLogWindowMessage(object sender, object target)
			: base(sender, target) {}
	}
}