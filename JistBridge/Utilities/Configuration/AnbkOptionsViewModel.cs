using System.ComponentModel;
using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Properties;

namespace JistBridge.Utilities.Configuration {
	[Export(typeof (IAnbkOptionsViewModel))]
	[DisplayName("ANBK Configuration")]
	internal class AnbkOptionsViewModel : IAnbkOptionsViewModel {
	}
}