using System;
using System.Net;

namespace SamFirm
{
    internal class KiesRequest : WebRequest
    {
        public static new HttpWebRequest Create(string requestUriString)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.Headers["Cache-Control"] = "no-cache";
            request.UserAgent = "Kies2.0_FUS";
            request.Headers.Add("Authorization", "FUS nonce=\"\", signature=\"\", nc=\"\", type=\"\", realm=\"\"");
            request.CookieContainer = Web.Cookies;
            return request;
        }
    }
}