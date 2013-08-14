using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Properties;

namespace JistBridge.Utilities.Configuration {
	[Export(typeof (IAppConfiguration))]
	public class AppConfiguration : IAppConfiguration {
		public string ApplicationName {
			get {
				//var test = Settings.Default.Test;
				//Settings.Default.Test = "Sham";
				//Settings.Default.Save();
				return Settings.Default.ApplicationName;
			}
		}
	}
}