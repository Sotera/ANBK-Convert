using JistBridge.Interfaces;
using JistBridge.Properties;
using System.ComponentModel.Composition;

namespace JistBridge.Utilities.Configuration
{
	[Export(typeof(IUserConfiguration))]
	public class UserConfiguration : IUserConfiguration
	{
		private string _testProperty = "Test Prop Val";

		public string TestProperty {
			get {
				return _testProperty;
			}
			set {
				_testProperty = value;
			}
		}
	}
}