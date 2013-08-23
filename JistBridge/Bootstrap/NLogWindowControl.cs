using System.ComponentModel.Composition;
using System.Windows;
using JistBridge.Interfaces;
using JistBridge.Messages;
using NLog;
using NLog.Config;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class NLogWindowControl : IBootstrapTask {
		internal NLogWindowControl() {
			ShowNLogWindowMessage.Register(this, msg => {
				var asyncWrapper = ShowNLogWindowMessage.AsyncTargetWrapper;
				var rtb = ShowNLogWindowMessage.RichTextBox;
				rtb.Visibility = Visibility.Visible;

				SimpleConfigurator.ConfigureForTargetLogging(asyncWrapper, LogLevel.Trace);
				new AddRemoveDocumentViewMessage(null, null, cbMsg => { }) {
					Operation = Operation.Add,
					ReportView = rtb,
					TabText = "NLog Output"
				}.Send();
			});
		}
	}
}