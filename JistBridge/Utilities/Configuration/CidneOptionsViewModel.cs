using System.ComponentModel;
using System.ComponentModel.Composition;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Properties;

// ReSharper disable once CSharpWarnings::CS0665

namespace JistBridge.Utilities.Configuration {
	[Export(typeof (ICidneOptionsViewModel))]
	[DisplayName("CIDNE Configuration")]
	internal class CidneOptionsViewModel : ICidneOptionsViewModel {
		//Avoid 'no "Name"' exception out of XCeed PropertyGrid control
		public string Name {
			get { return ""; }
		}

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
		[DisplayName("Queue Report")]
		[Description("URL for the QueueReport ReST call.")]
		public string QueueReportUrl {
			get { return Settings.Default.QueueReportUrl; }
			set {
				Settings.Default.QueueReportUrl = value;
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

		[Category("ReST Services URLs")]
		[DisplayName("Get Metadata Schema")]
		[Description("URL for the GetMetadataSchema ReST call.")]
		public string GetMetadataSchemasUrl {
			get { return Settings.Default.GetMetadataSchemasUrl; }
			set {
				Settings.Default.GetMetadataSchemasUrl = value;
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
				if (Settings.Default.EnableGetReportPolling = value) new GetReportRestMessage(null, null).Send();
				Settings.Default.Save();
			}
		}
	}
}