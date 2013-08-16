using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Utilities.DialogManagement;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class PropertyEditorControl : IBootstrapTask {
		internal PropertyEditorControl() {
			ShowPropertyEditorMessage.Register(this, msg => new ShowModalDialogMessage(msg.Sender, msg.Target) {
				Type = ShowModalDialogMessage.DialogType.Custom,
				ContainedControl = new PropertyGrid {
					SelectedObject = msg.PropertiesObject,
					Width = 640,
					Height = 480
				},
				DialogMode = DialogMode.Ok
			}.Send());
		}
	}
}