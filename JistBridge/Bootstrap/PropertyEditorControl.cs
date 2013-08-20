using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI.ToolsOptions;
using JistBridge.Utilities.DialogManagement;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class PropertyEditorControl : IBootstrapTask {
		[Import(typeof (ToolsOptions))]
		internal ToolsOptions ToolsOptions { get; set; }

		internal PropertyEditorControl() {
			ShowPropertyEditorMessage.Register(this, msg => new ShowModalDialogMessage(msg.Sender, msg.Target) {
				Type = ShowModalDialogMessage.DialogType.Custom,
				ContainedControl = ToolsOptions,
				DialogMode = DialogMode.Ok
			}.Send());
		}
	}
}