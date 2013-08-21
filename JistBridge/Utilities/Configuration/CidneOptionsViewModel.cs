using System.ComponentModel;
using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Properties;

// ReSharper disable once CSharpWarnings::CS0665

namespace JistBridge.Utilities.Configuration {
	[Export(typeof (ICidneOptionsViewModel))]
	internal class CidneOptionsViewModel : ICidneOptionsViewModel {
		[Category("ReST Services URLs")]
		[DisplayName("Validate User")]
		[Description("URL for the ValidateUser ReST call.")]
		public string ValidateUserUrl {
			get { return Settings.Default.ValidateUserUrl; }
			set {
				Settings.Default.ValidateUserUrl = value;
				Settings.Default.Save();
			}
		}

		[Category("ReST Services URLs")]
		[DisplayName("Get Report")]
		[Description("URL for the GetReport ReST call.")]
		public string GetReportUrl {
			get { return Settings.Default.GetReportUrl; }
			set {
				Settings.Default.GetReportUrl = value;
				Settings.Default.Save();
			}
		}

		[Category("ReST Services URLs")]
		[DisplayName("Save Report")]
		[Description("URL for the SaveReport ReST call.")]
		public string SaveReportUrl {
			get { return Settings.Default.SaveReportUrl; }
			set {
				Settings.Default.SaveReportUrl = value;
				Settings.Default.Save();
			}
		}

		[Category("GetReport Service Polling")]
		[DisplayName("Polling Delay (ms)")]
		[Description("Time between calls to GetReport ReST service, in milliseconds.")]
		public int GetReportPollDelayMS {
			get { return Settings.Default.GetReportPollDelayMS; }
			set {
				Settings.Default.GetReportPollDelayMS = value;
				Settings.Default.Save();
			}
		}

		[Category("GetReport Service Polling")]
		[DisplayName("Enable Polling")]
		[Description("Turns GetReport ReST service polling on and off.")]
		public bool EnableGetReportPolling {
			get { return Settings.Default.EnableGetReportPolling; }
			set {
				if (Settings.Default.EnableGetReportPolling = value) {
					new GetReportRestMessage(null, null).Send();
				}
				Settings.Default.Save();
			}
		}
	}
}