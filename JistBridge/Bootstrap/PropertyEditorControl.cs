using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.UI.ToolsOptions;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class PropertyEditorControl : IBootstrapTask {
		[Import(typeof (ToolsOptions))]
		internal ToolsOptions ToolsOptions { get; set; }

		internal PropertyEditorControl() {
			ShowOptionsDialogMessage.Register(this, msg => new ShowDialogMessage(msg.Sender, msg.Target) {
				Title = "Options",
				IsModal = true,
				ContainedControl = ToolsOptions,
			}.Send());
		}
	}
}