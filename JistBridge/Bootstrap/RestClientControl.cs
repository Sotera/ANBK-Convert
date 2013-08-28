using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using RestSharp;

namespace JistBridge.Bootstrap
{
    [Export(typeof(IBootstrapTask))]
    internal class RestClientControl : IBootstrapTask
    {
        private ICidneOptionsViewModel _cidneOptions;

        [Import]
        internal ICidneOptionsViewModel CidneOptions
        {
            get { return _cidneOptions; }
            set
            {
                _cidneOptions = value;
                //Check to see if we ought to start polling our CIDNE server
                if (CidneOptions.EnableGetReportPolling)
                {
                    new GetReportRestMessage(null, null).Send();
                }
            }
        }

        internal RestClientControl()
        {
            ValidateUserRestMessage.Register(this, ValidateUserRestMessageHandler);
            GetReportRestMessage.Register(this, GetReportRestMessageHandler);
            SaveReportRestMessage.Register(this, SaveReportRestMessageHandler);
        }

        private void GetReportRestMessageHandler(GetReportRestMessage msg)
        {
            GetRestResponse<GetReportResponse>(CidneOptions.GetReportUrl, "",
                res =>
                {
                    if (res.Data != null)
                    {
                        new ReportReceivedMessage(null, null) { GetReportResponse = res.Data }.Send();
                    }
                    if (CidneOptions.EnableGetReportPolling)
                    {
                        new GetReportRestMessage(null, null).SendAfterWaiting(CidneOptions.GetReportPollDelayMS);
                    }
                });
        }

        private void SaveReportRestMessageHandler(SaveReportRestMessage msg)
        {
            GetRestResponse<SaveReportResponse>(CidneOptions.SaveReportUrl, "",
                res =>
                {
                    if (res.Data != null)
                    {
                        new ReportSavedMessage(null, null) { SaveReportResponse = res.Data }.Send();
                    }
                }, msg.ReportData.report);
        }

        private void ValidateUserRestMessageHandler(ValidateUserRestMessage msg)
        {
            //var retVal = GetRestResponse<ValidateUserResponse>(CidneOptions.ValidateUserUrl, "ValidateUser");
        }

        private static void GetRestResponse<T>(string url, string rootElement,
            Action<RestResponse<T>> cb, object bodyData = null) where T : new()
        {
            try
            {
                var uri = new Uri(url);
                var restClient = new RestClient
                {
                    BaseUrl = uri.Scheme + "://" + uri.Authority
                };
                var restRequest = new RestRequest
                {
                    Method = Method.POST,
                    RootElement = rootElement,
                    Resource = uri.AbsolutePath,
                    RequestFormat = DataFormat.Json

                };
                if (bodyData != null) restRequest.AddBody(bodyData);
                restClient.ExecuteAsync<T>(restRequest, (res, handle) => cb((RestResponse<T>)res));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}