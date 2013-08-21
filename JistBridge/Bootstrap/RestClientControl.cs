using System;
using System.ComponentModel.Composition;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using RestSharp;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class RestClientControl : IBootstrapTask {
		private ICidneOptionsViewModel _cidneOptions;

		[Import]
		internal ICidneOptionsViewModel CidneOptions {
			get { return _cidneOptions; }
			set {
				_cidneOptions = value;
				//Check to see if we ought to start polling our CIDNE server
				if (CidneOptions.EnableGetReportPolling) {
					new GetReportRestMessage(null, null).Send();
				}
			}
		}

		internal RestClientControl() {
			ValidateUserRestMessage.Register(this, ValidateUserRestMessageHandler);
			GetReportRestMessage.Register(this, GetReportRestMessageHandler);
		}

		private void GetReportRestMessageHandler(GetReportRestMessage msg) {
			var getReportResponse = GetRestResponse<GetReportResponse>(CidneOptions.GetReportUrl, "");
			if (getReportResponse != null) {
				new ReportReceivedMessage(null, null){GetReportResponse = getReportResponse}.Send();
			}
			if (CidneOptions.EnableGetReportPolling) {
				new GetReportRestMessage(null, null).SendAfterWaiting(CidneOptions.GetReportPollDelayMS);
			}
		}

		private void ValidateUserRestMessageHandler(ValidateUserRestMessage msg) {
			var retVal = GetRestResponse<ValidateUserResponse>(CidneOptions.ValidateUserUrl, "ValidateUser");
		}

		private T GetRestResponse<T>(string url, string rootElement) where T : new() {
			var uri = new Uri(url);
			var restClient = new RestClient {
				BaseUrl = uri.Scheme + "://" + uri.Authority
			};
			var restRequest = new RestRequest {
				Method = Method.POST,
				RootElement = rootElement,
				Resource = uri.AbsolutePath
			};
			return restClient.Execute<T>(restRequest).Data;
		}
	}
}