using System;
using System.ComponentModel.Composition;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using RestSharp;

namespace JistBridge.Bootstrap {
	[Export(typeof (IBootstrapTask))]
	internal class RestClientControl : IBootstrapTask {
		[Import]
		internal IUserConfiguration UserConfiguration { get; set; }

		internal RestClientControl() {
			ValidateUserRestMessage.Register(this, ValidateUserRestMessageHandler);
			GetReportRestMessage.Register(this, GetReportRestMessageHandler);
		}

		private void GetReportRestMessageHandler(GetReportRestMessage msg) {
			var retVal = GetRestResponse<GetReportResponse>(UserConfiguration.GetReportUrl, "");
		}

		private void ValidateUserRestMessageHandler(ValidateUserRestMessage msg) {
			var retVal = GetRestResponse<ValidateUserResponse>(UserConfiguration.ValidateUserUrl, "ValidateUser");
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