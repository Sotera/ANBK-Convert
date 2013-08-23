using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Utilities.NLogWpfRichTextTarget;
using NLog.Targets.Wrappers;

namespace JistBridge.UI.MainWindow {
	public partial class MainWindow {
		public MainWindow() {
			InitializeComponent();
			ShowModalDialogMessage.SetDefaultContentControl(this);
			new QueueMefComposeMessage(null, null, this, null).Send();
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs) {
			var target = new WpfRichTextBoxTarget {
				Name = "console",
				Layout = "${longdate:useUTC=true}|${level:uppercase=true}|${logger}::${message}",
				ControlName = "RtbNLogWindow",
				FormName = "MainWindowName",
				AutoScroll = true,
				MaxLines = 100000,
				UseDefaultRowColoringRules = true
			};
			var asyncWrapper = new AsyncTargetWrapper {Name = "console", WrappedTarget = target};
			//SimpleConfigurator.ConfigureForTargetLogging(asyncWrapper, LogLevel.Trace);

			var childElements = ((Panel) Content).Children;
			var nLogControl = childElements.Cast<Control>().Single(c => c.Name == "RtbNLogWindow");
			childElements.Remove(nLogControl);

			ShowNLogWindowMessage.SetNLogWindowInfo(asyncWrapper, nLogControl as System.Windows.Controls.RichTextBox);
		}

		[Import]
		public IMainWindowViewModel MainWindowViewModel {
			set {
				value.SetLayoutDocumentInfo(LayoutRoot, LayoutDocumentPaneGroup);
				DataContext = value;
			}
		}
	}
}