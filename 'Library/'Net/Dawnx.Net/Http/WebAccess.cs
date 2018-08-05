﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Dawnx.Net.Http
{
    public class WebAccess
    {
        static WebAccess()
        {
            //TODO: If WebClient can run in netcore 2.1+ normally, delete it.
            AppContext.SetSwitch("System.Net.Http.UseSocketsHttpHandler", false);
        }

        public const int RECOMMENDED_BUFFER_SIZE = 256 * 1024;      // 256 KB

        public delegate void ProgressHandler(WebAccess sender, string url, long done, long length);
        public event ProgressHandler DownloadProgress;
        public event ProgressHandler UploadProgress;

        public WebRequestStateContainer StateContainer { get; private set; }
        public HashSet<IResponseProcessor> ResponseProcessors { get; private set; }
            = new HashSet<IResponseProcessor>().Self(_ => _.Add(new RedirectProcessor()));

        public WebAccess() : this(new WebRequestStateContainer()) { }
        public WebAccess(WebRequestStateContainer config)
        {
            if (config != null)
                StateContainer = config;
            else StateContainer = new WebRequestStateContainer();
        }

        public void AttachProcessor(IResponseProcessor processor)
            => ResponseProcessors.Add(processor);
        public void ClearProcessor() => ResponseProcessors.Clear();

        public int AllowRedirectTimes { get; set; } = 10;
        public int RedirectTimes { get; set; } = 0;

        public string Get(string url, Dictionary<string, object> updata = null)
            => ReadString(WebRequestStateContainer.GET, url, WebRequestStateContainer.URL_ENCODED, updata, null);
        public string Post(string url, Dictionary<string, object> updata = null)
            => ReadString(WebRequestStateContainer.POST, url, WebRequestStateContainer.URL_ENCODED, updata, null);
        public string Up(string url, Dictionary<string, object> updata = null, Dictionary<string, object> upfiles = null)
            => ReadString(WebRequestStateContainer.POST, url, WebRequestStateContainer.FORM_DATA, updata, upfiles);

        public void GetDownload(Stream receiver, string url, Dictionary<string, object> updata = null,
            int bufferSize = RECOMMENDED_BUFFER_SIZE)
            => Download(receiver, WebRequestStateContainer.GET, url, WebRequestStateContainer.URL_ENCODED, updata, null, bufferSize);
        public void PostDownload(Stream receiver, string url, Dictionary<string, object> updata = null,
            int bufferSize = RECOMMENDED_BUFFER_SIZE)
            => Download(receiver, WebRequestStateContainer.POST, url, WebRequestStateContainer.URL_ENCODED, updata, null, bufferSize);
        public void UpDownload(Stream receiver, string url, Dictionary<string, object> updata = null, Dictionary<string, object> upfiles = null,
            int bufferSize = RECOMMENDED_BUFFER_SIZE)
            => Download(receiver, WebRequestStateContainer.POST, url, WebRequestStateContainer.FORM_DATA, updata, upfiles, bufferSize);

        public void Download(
            Stream receiver,
            string method, string url, string enctype,
            Dictionary<string, object> updata,
            Dictionary<string, object> upfiles,
            int bufferSize)
        {
            using (var response = GetResponse(method, url, enctype, updata, upfiles))
            {
                long received = 0;
                using (var stream = response.GetResponseStream())
                {
                    stream.ReadProcessing(bufferSize, (buffer, readLength) =>
                    {
                        receiver.Write(buffer, 0, readLength);
                        received += readLength;
                        DownloadProgress?.Invoke(this, url, received, response.ContentLength);
                    });
                }
            }
        }

        public string ReadString(
            string method, string url, string enctype,
            Dictionary<string, object> updata,
            Dictionary<string, object> upfiles)
        {
            using (var response = GetResponse(method, url, enctype, updata, upfiles))
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public HttpWebResponse GetResponse(
            string method, string url, string enctype,
            Dictionary<string, object> updata,
            Dictionary<string, object> upfiles)
        {
            var response = GetPureResponse(method, url, enctype, updata, upfiles);

            foreach (Cookie respCookie in response.Cookies)
                StateContainer.Cookies.Add(respCookie);

            foreach (var processor in ResponseProcessors)
            {
                var processedResponse = processor.Process(this, response, method, url, enctype, updata, upfiles);

                if (processedResponse != null)
                    return processedResponse;
                else continue;
            }

            return response;
        }

        public HttpWebResponse GetPureResponse(
            string method, string url, string enctype,
            Dictionary<string, object> updata,
            Dictionary<string, object> upfiles)
        {
            method = method.ToLower();
            enctype = enctype.ToLower();

            if (updata == null)
                updata = new Dictionary<string, object>();
            if (upfiles == null)
                upfiles = new Dictionary<string, object>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding(StateContainer.Encoding);

            var contentType = enctype;
            byte[] body;

            switch (enctype)
            {
                default:
                case WebRequestStateContainer.URL_ENCODED:
                    var bodyList = new List<string>();
                    foreach (var data in updata)
                    {
                        var values = NormalizeStringValues(data.Value);
                        foreach (var value in values)
                            bodyList.Add($"{data.Key}={HttpUtility.UrlEncode(value)}");
                    }

                    body = encoding.GetBytes(string.Join("&", bodyList));

                    if (method == WebRequestStateContainer.GET)
                    {
                        if (!url.Contains("?"))
                            url = $"{url}?{encoding.GetString(body)}";
                        else url = $"{url}&{encoding.GetString(body)}";
                    }
                    break;

                case WebRequestStateContainer.FORM_DATA:
                    var formData = new HttpFormData(encoding);
                    foreach (var data in updata)
                    {
                        var values = NormalizeStringValues(data.Value);
                        foreach (var value in values)
                            formData.AddData(data.Key, encoding.GetBytes(value.ToString()));
                    }
                    foreach (var file in upfiles)
                    {
                        var values = NormalizeStringValues(file.Value);
                        foreach (var value in values)
                            formData.AddFile(file.Key, Path.GetFileName(value), File.ReadAllBytes(value));
                    }

                    body = formData;
                    contentType = formData.ContentType;
                    break;
            }

            var request = ((HttpWebRequest)WebRequest.Create(new Uri(url))).Self(_ =>
            {
                _.ContentType = contentType;
                _.UserAgent = StateContainer.UserAgent;
                _.Method = method;
                _.Timeout = -1;
                _.UseDefaultCredentials = StateContainer.SystemLogin;
                if (StateContainer.UseProxy)
                {
                    if (!string.IsNullOrEmpty(StateContainer.ProxyAddress))
                    {
                        _.Proxy = new WebProxy
                        {
                            Address = new Uri(StateContainer.ProxyAddress),
                            Credentials = new NetworkCredential
                            {
                                UserName = StateContainer.ProxyUsername,
                                Password = StateContainer.ProxyPassword,
                            }
                        };
                    }
                }
                else _.Proxy = null;

                _.CookieContainer = StateContainer.Cookies;
            });

            if (method == WebRequestStateContainer.POST)
            {
                request.ContentLength = body.Length;
                int send = 0;
                using (var stream = request.GetRequestStream())
                {
                    int sendLength = body.Length - send;
                    if (sendLength > RECOMMENDED_BUFFER_SIZE)
                        sendLength = RECOMMENDED_BUFFER_SIZE;

                    stream.Write(body, send, sendLength);
                    send += sendLength;
                    UploadProgress?.Invoke(this, url, send, body.Length);
                }
            }

            HttpWebResponse response = null;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                response = ex.Response as HttpWebResponse;
                if (response == null) throw;
            }
            return response;
        }

        private static IEnumerable<string> NormalizeStringValues(object dvalue)
        {
            if (dvalue is Array)
            {
                return (dvalue as Array).OfType<object>()
                    .Select(value => value.ToString()).ToArray();
            }
            else return new[] { dvalue?.ToString() ?? "" };
        }

    }
}