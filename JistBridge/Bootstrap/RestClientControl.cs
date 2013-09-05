using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Threading;
using JistBridge.Data.ReST;
using JistBridge.Interfaces;
using JistBridge.Messages;
using JistBridge.Messages.ANBK;
using JistBridge.Properties;
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
                    new GetReportRestMessage(null, null).Send();
            }
        }

        internal RestClientControl()
        {
            ValidateUserRestMessage.Register(this, ValidateUserRestMessageHandler);
            GetReportRestMessage.Register(this, GetReportRestMessageHandler);
            QueueReportRestMessage.Register(this, QueueReportRestMessageHandler);
            SaveReportRestMessage.Register(this, SaveReportRestMessageHandler);
            GetMetadataSchemasRestMessage.Register(this, GetMetadataSchemasRestMessageHandler);
        }

        private void GetReportRestMessageHandler(GetReportRestMessage msg)
        {
            GetRestResponse<GetReportResponse>(CidneOptions.GetReportUrl, "GetReport",
                res =>
                {
                    if (res.Data != null)
                        if (res.Data.resultCode != 1)
                        {
                            DispatcherHelper.UIDispatcher.Invoke(() => new ShowDialogMessage(null, null)
                            {
                                Title = "GetReport Failed",
                                IsModal = true,
                                ContainedControl = new Label { Content = res.Data.description, Height = 40 },
                            }.Send());
                            return;
                        }
                    new ReportReceivedMessage(null, null)
                    {
                        GetReportResponse = res.Data
                    }.Send();
                    if (CidneOptions.EnableGetReportPolling)
                        new GetReportRestMessage(null, null).SendAfterWaiting(CidneOptions.GetReportPollDelayMS);
                },
                restRequest =>
                {
                    restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
                    var userPoid = UserCredentialsControl.UserCredentials.UserInfo.poid;
                    restRequest.AddParameter("proxy_ticket", userPoid, ParameterType.GetOrPost);
                });
        }

        private void QueueReportRestMessageHandler(QueueReportRestMessage msg)
        {
            GetRestResponse<QueueReportResponse>(CidneOptions.QueueReportUrl, "QueueReport",
                res => DispatcherHelper.UIDispatcher.Invoke(() => new ShowDialogMessage(msg.Sender, msg.Target)
                {
                    Title = "Queue Report Response",
                    IsModal = true,
                    ContainedControl = new Label { Content = res.Data.description },
                }.Send()),
                restRequest =>
                {
                    restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
                    var userPoid = UserCredentialsControl.UserCredentials.UserInfo.poid;
                    restRequest.AddParameter("proxy_ticket", userPoid, ParameterType.GetOrPost);
                    restRequest.AddParameter("resourceId", Settings.Default.ResourceId, ParameterType.GetOrPost);
                });
        }

        private void SaveReportRestMessageHandler(SaveReportRestMessage msg)
        {
            GetRestResponse<SaveReportResponse>(CidneOptions.SaveReportUrl, "",
                res => new ReportSavedMessage(null, null)
                {
                    SaveReportResponse = res.Data
                }.Send(),
                restRequest =>
                {
                    var filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                   "\\JISTBridge\\ANBK.anb";
                    using (var fs = File.OpenRead(filename))
                    {
                        var filebytes = new byte[fs.Length];
                        fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                        var encodedData = Convert.ToBase64String(filebytes,                 
                                                                   Base64FormattingOptions.InsertLineBreaks);
                        msg.ReportData.report.diagram = encodedData;
                        restRequest.AddBody(msg.ReportData.report);
                    }
                    
                });
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            /*if (!string.IsNullOrEmpty(txtOutFile.Text))
            {
                byte[] filebytes = Convert.FromBase64String(txtEncoded.Text);
                FileStream fs = new FileStream(txtOutFile.Text,
                                               FileMode.CreateNew,
                                               FileAccess.Write,
                                               FileShare.None);
                fs.Write(filebytes, 0, filebytes.Length);
                fs.Close();
            }*/
        }

        private void ValidateUserRestMessageHandler(ValidateUserRestMessage msg)
        {
            GetRestResponse<ValidateUserResponse>(CidneOptions.ValidateUserUrl, "ValidateUser",
                res => new UserCredentialsMessage(this, null) { UserCredentials = res.Data }.Send(),
                restRequest =>
                {
                    restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
                    restRequest.AddParameter("password", "password", ParameterType.GetOrPost);
                });
        }

        private void GetMetadataSchemasRestMessageHandler(GetMetadataSchemasRestMessage msg)
        {
            GetRestResponse<GetMetadataSchemasResponse>(CidneOptions.GetMetadataSchemasUrl, "GetMetadataSchemas",
                res => { if (res.Data != null) { } },
                restRequest =>
                {
                    restRequest.AddParameter("username", "admin", ParameterType.GetOrPost);
                    var userPoid = UserCredentialsControl.UserCredentials.UserInfo.poid;
                    restRequest.AddParameter("proxy_ticket", userPoid, ParameterType.GetOrPost);
                });
        }

        private static void GetRestResponse<T>(string url, string rootElement,
            Action<RestResponse<T>> cb, Action<RestRequest> cbRestRequest = null) where T : new()
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
                if (cbRestRequest != null) cbRestRequest(restRequest);
                restClient.ExecuteAsync<T>(restRequest, (res, handle) =>
                {
                    if (res.ResponseStatus != ResponseStatus.Completed)
                    {
                        DispatcherHelper.UIDispatcher.Invoke(() => new ShowDialogMessage(null, null)
                        {
                            Title = "GetReport Failed",
                            IsModal = true,
                            ContainedControl = new Label { Content = res.ErrorMessage, Height = 40 },
                        }.Send());
                        return;
                    }
                    cb((RestResponse<T>)res);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}