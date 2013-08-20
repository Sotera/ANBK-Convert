using System.ComponentModel.Composition;
using JistBridge.Interfaces;

namespace JistBridge.UI.ToolsOptions {
	[Export(typeof (ToolsOptions))]
	public partial class ToolsOptions {
		[Import(typeof (IToolsOptionsViewModel))]
		internal IToolsOptionsViewModel ToolsOptionsViewModel {
			set { DataContext = value; }
		}

		public ToolsOptions() {
			InitializeComponent();
		}
	}
}