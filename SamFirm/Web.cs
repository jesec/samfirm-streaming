using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace SamFirm
{
    internal static class Web
    {
        public static string JSessionID { get; set; } = string.Empty;
        public static string Nonce { get; set; } = string.Empty;

        public static int DownloadBinary(string path, string file, string saveTo, string size)
        {
            long num = 0L;
            HttpWebRequest wr = KiesRequest.Create("http://cloud-neofussvr.sslcs.cdngc.net/NF_DownloadBinaryForMass.do?file=" + path + file);
            wr.Method = "GET";
            wr.Headers["Authorization"] = Imports.GetAuthorization(Nonce).Replace("Authorization: ", "").Replace("nonce=\"", "nonce=\"" + Nonce);
            wr.Timeout = 0x61a8;
            wr.ReadWriteTimeout = 0x61a8;
            using (HttpWebResponse response = (HttpWebResponse) wr.GetFUSResponse())
            {
                if (response == null)
                {
                    Console.WriteLine("Error DownloadBinary(): response is null.");
                    return 0x385;
                }
                if ((response.StatusCode != HttpStatusCode.OK) && (response.StatusCode != HttpStatusCode.PartialContent))
                {
                    Console.WriteLine("Error DownloadBinary(): " + ((int)response.StatusCode));
                }
                else
                {
                    long total = long.Parse(response.GetResponseHeader("content-length")) + num;
                    byte[] buffer = new byte[0x2000];
                    try
                    {
                        Utility.PreventDeepSleep(Utility.PDSMode.Start);
                        Decrypt.DecryptFile(response.GetResponseStream(), saveTo);
                    }
                    catch (Exception exception)
                    {
                        Logger.WriteLine("Error DownloadBinary(): ");
                        Logger.WriteLine(exception.ToString());
                        return -1;
                    }
                    finally
                    {
                        Utility.PreventDeepSleep(Utility.PDSMode.Stop);
                    }
                }
                return 0;
            }
        }

        public static int DownloadBinaryInform(string xml, out string xmlresponse) => 
            XMLFUSRequest("https://neofussvr.sslcs.cdngc.net/NF_DownloadBinaryInform.do", xml, out xmlresponse);

        public static int DownloadBinaryInit(string xml, out string xmlresponse) => 
            XMLFUSRequest("https://neofussvr.sslcs.cdngc.net/NF_DownloadBinaryInitForMass.do", xml, out xmlresponse);

        public static int GenerateNonce()
        {
            HttpWebRequest wr = KiesRequest.Create("https://neofussvr.sslcs.cdngc.net/NF_DownloadGenerateNonce.do");
            wr.Method = "POST";
            wr.ContentLength = 0L;
            using (HttpWebResponse response = (HttpWebResponse) wr.GetFUSResponse())
            {
                if (response == null)
                {
                    return 0x385;
                }
                return (int) response.StatusCode;
            }
        }

        public static void SetReconnect()
        {

        }

        private static int XMLFUSRequest(string URL, string xml, out string xmlresponse)
        {
            xmlresponse = null;
            HttpWebRequest wr = KiesRequest.Create(URL);
            wr.Method = "POST";
            wr.Headers["Authorization"] = "FUS nonce=\"\", signature=\"" + Imports.GetAuthorization(Nonce) + "\", nc=\"\", type=\"\", realm=\"\"";
            byte[] bytes = Encoding.ASCII.GetBytes(Regex.Replace(xml, @"\r\n?|\n|\t", string.Empty));
            wr.ContentLength = bytes.Length;
            using (Stream stream = wr.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse) wr.GetFUSResponse())
            {
                if (response == null)
                {
                    return 0x385;
                }
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    try
                    {
                        xmlresponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    }
                    catch (Exception)
                    {
                        return 900;
                    }
                }
                return (int) response.StatusCode;
            }
        }

        public static WebResponse GetFUSResponse(this WebRequest wr)
        {
            try
            {
                WebResponse response = wr.GetResponse();
                if (response.Headers.AllKeys.Contains("Set-Cookie"))
                {
                    JSessionID = response.Headers["Set-Cookie"].Replace("JSESSIONID=", "").Split(new[] { ';' })[0];
                }
                if (response.Headers.AllKeys.Contains("NONCE"))
                {
                    Nonce = response.Headers["NONCE"];
                }
                return response;
            }
            catch (WebException exception)
            {
                Logger.WriteLine("Error GetResponseFUS() -> " + exception.ToString());
                if (exception.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    SetReconnect();
                }
                return exception.Response;
            }
        }
    }
}