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
				if (CidneOptions.EnableGetReportPolling)
					new GetReportRestMessage(null, null).Send();
			}
		}

		internal RestClientControl() {
			ValidateUserRestMessage.Register(this, ValidateUserRestMessageHandler);
			GetReportRestMessage.Register(this, GetReportRestMessageHandler);
			QueueReportRestMessage.Register(this, QueueReportRestMessageHandler);
			SaveReportRestMessage.Register(this, SaveReportRestMessageHandler);
			GetMetadataSchemasRestMessage.Register(this, GetMetadataSchemasRestMessageHandler);
		}

		private void GetReportRestMessageHandler(GetReportRestMessage msg) {
			GetRestResponse<GetReportResponse>(CidneOptions.GetReportUrl, "GetReport",
				res => {
					if (res.Data != null)
						new ReportReceivedMessage(null, null) {GetReportResponse = res.Data}.Send();
					if (CidneOptions.EnableGetReportPolling)
						new GetReportRestMessage(null, null).SendAfterWaiting(CidneOptions.GetReportPollDelayMS);
				},
				restRequest => {
					restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
					var userPoid = UserCredentialsControl.UserCredentials.UserInfo.poid;
					restRequest.AddParameter("proxy_ticket", userPoid, ParameterType.GetOrPost);
				});
		}

		private void QueueReportRestMessageHandler(QueueReportRestMessage msg) {
			GetRestResponse<QueueReportResponse>(CidneOptions.QueueReportUrl, "QueueReport",
				res => {
					if (res.Data != null) {
						var rrr = res.Data;
						rrr = res.Data;
					}
				},
				restRequest => {
					restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
					var userPoid = UserCredentialsControl.UserCredentials.UserInfo.poid;
					restRequest.AddParameter("proxy_ticket", userPoid, ParameterType.GetOrPost);
					restRequest.AddParameter("resourceId", "9E7A9010-E7D2-87C8-BA09B614B63D8006", ParameterType.GetOrPost);
				});
		}

		private void SaveReportRestMessageHandler(SaveReportRestMessage msg) {
			GetRestResponse<SaveReportResponse>(CidneOptions.SaveReportUrl, "",
				res => new ReportSavedMessage(null, null) {SaveReportResponse = res.Data}.Send(),
				restRequest => restRequest.AddBody(msg.ReportData.report));
		}

		private void ValidateUserRestMessageHandler(ValidateUserRestMessage msg) {
			GetRestResponse<ValidateUserResponse>(CidneOptions.ValidateUserUrl, "ValidateUser",
				res => new UserCredentialsMessage(this, null) {UserCredentials = res.Data}.Send(),
				restRequest => {
					restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
					restRequest.AddParameter("password", "password", ParameterType.GetOrPost);
				});
		}

		private void GetMetadataSchemasRestMessageHandler(GetMetadataSchemasRestMessage msg) {
			GetRestResponse<GetMetadataSchemasResponse>(CidneOptions.GetMetadataSchemasUrl, "GetMetadataSchemas",
				res => {
					if (res.Data != null) {
						var rrr = res.Data;
						rrr = res.Data;
					}
				},
				restRequest => {
					restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
					var userPoid = UserCredentialsControl.UserCredentials.UserInfo.poid;
					restRequest.AddParameter("proxy_ticket", userPoid, ParameterType.GetOrPost);
				});
		}

		private static void GetRestResponse<T>(string url, string rootElement,
			Action<RestResponse<T>> cb, Action<RestRequest> cbRestRequest = null) where T : new() {
			try {
				var uri = new Uri(url);
				var restClient = new RestClient {
					BaseUrl = uri.Scheme + "://" + uri.Authority
				};
				var restRequest = new RestRequest {
					Method = Method.POST,
					RootElement = rootElement,
					Resource = uri.AbsolutePath,
					RequestFormat = DataFormat.Json
				};
				if (cbRestRequest != null) cbRestRequest(restRequest);
				restClient.ExecuteAsync<T>(restRequest, (res, handle) => cb((RestResponse<T>) res));
			}
			catch (Exception e) {
				Console.WriteLine(e);
			}
		}
	}
}