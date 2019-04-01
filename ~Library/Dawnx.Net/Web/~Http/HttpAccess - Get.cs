﻿using Dawnx.Definition;
using Dawnx.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Dawnx.Net.Web
{
    public partial class HttpAccess
    {
        public string Get(string url, Dictionary<string, object> updata = null)
        {
            return ReadString(
                HttpVerb.GET, MimeType.APPLICATION_X_WWW_FORM_URLENCODED,
                url, updata, null);
        }
        public string Get(string url, object updata) => Get(url, ObjectUtility.CovertToDictionary(updata));

        public string GetDownload(Stream receiver, string url, Dictionary<string, object> updata = null, int bufferSize = RECOMMENDED_BUFFER_SIZE)
            => Download(receiver, HttpVerb.GET, MimeType.APPLICATION_X_WWW_FORM_URLENCODED, url, updata, null, bufferSize);
        public string GetDownload(Stream receiver, string url, object updata, int bufferSize = RECOMMENDED_BUFFER_SIZE)
            => GetDownload(receiver, url, ObjectUtility.CovertToDictionary(updata), bufferSize);

        public TRet GetFor<TRet>(string url, Dictionary<string, object> updata = null)
            => JsonConvert.DeserializeObject<TRet>(Get(url, updata));
        public TRet GetFor<TRet>(string url, object updata)
            => GetFor<TRet>(url, ObjectUtility.CovertToDictionary(updata));

        public JToken GetFor(string url, Dictionary<string, object> updata = null)
            => JsonConvert.DeserializeObject<JToken>(Get(url, updata));
        public JToken GetFor(string url, object updata)
            => GetFor(url, ObjectUtility.CovertToDictionary(updata));

        public Stream GetStreamUsingGet(string url, Dictionary<string, object> updata = null)
        {
            var resp = GetPureResponse(HttpVerb.GET, MimeType.APPLICATION_X_WWW_FORM_URLENCODED, url, updata, null);
            return resp.GetResponseStream();
        }

    }
}
