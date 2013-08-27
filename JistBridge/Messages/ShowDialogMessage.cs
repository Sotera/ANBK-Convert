using System.Windows.Controls;
using Xceed.Wpf.Toolkit.Primitives;

namespace JistBridge.Messages {
	public class ShowDialogMessage : BaseMessage<ShowDialogMessage> {
		private static WindowContainer _windowContainer;

		public Control ContainedControl { get; set; }
		public bool IsModal { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }

		public static void SetWindowContainer(WindowContainer windowContainer) {
			_windowContainer = windowContainer;
		}

		public WindowContainer WindowContainer {
			get { return _windowContainer; }
		}

		public ShowDialogMessage(object sender, object target)
			: base(sender, target) {
			Title = "";
			Text = "";
		}
	}
}