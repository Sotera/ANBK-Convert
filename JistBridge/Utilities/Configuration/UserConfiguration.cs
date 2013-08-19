using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Properties;

namespace JistBridge.Utilities.Configuration {
	[Export(typeof (IUserConfiguration))]
	public class UserConfiguration : IUserConfiguration {
		public string ValidateUserUrl {
			get { return Settings.Default.ValidateUserUrl; }
			set {
				Settings.Default.ValidateUserUrl = value;
				Settings.Default.Save();
			}
		}
		public string GetReportUrl {
			get { return Settings.Default.GetReportUrl; }
			set {
				Settings.Default.GetReportUrl = value;
				Settings.Default.Save();
			}
		}
		public string SaveReportUrl {
			get { return Settings.Default.SaveReportUrl; }
			set {
				Settings.Default.SaveReportUrl = value;
				Settings.Default.Save();
			}
		}
	}
}