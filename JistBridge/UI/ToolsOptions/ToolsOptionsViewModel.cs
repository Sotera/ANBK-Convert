using System.ComponentModel.Composition;
using JistBridge.Interfaces;

namespace JistBridge.UI.ToolsOptions {
	[Export(typeof (IToolsOptionsViewModel))]
	internal class ToolsOptionsViewModel : IToolsOptionsViewModel {
		[Import(typeof(ICidneOptionsViewModel ))]
		public ICidneOptionsViewModel CidneOptionsViewModel { get; set; }

		[Import(typeof(IAnbkOptionsViewModel ))]
		public IAnbkOptionsViewModel AnbkOptionsViewModel { get; set; }
	}
}