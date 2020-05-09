using System;
using System.Collections.Generic;
using System.Net;

namespace SamFirm
{
    internal static class FWFetch
    {
        internal static readonly List<Func<string, string, string>> FWFetchFuncs;

        static FWFetch()
        {
            List<Func<string, string, string>> list = new List<Func<string, string, string>> {
                new Func<string, string, string>(FOTAInfoFetch1),
            };
            FWFetchFuncs = list;
        }

        //http://fota-cloud-dn.ospserver.net
        public static string FOTAInfoFetch(string model, string region, bool latest)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xml = client.DownloadString("http://fota-cloud-dn.ospserver.net/firmware/" + region + "/" + model + "/version.xml");
                    string str2;
                    if (latest == true)
                    {
                        str2 = Xml.GetXMLValue(xml, "firmware/version/latest", null, null).ToUpper();
                    }
                    else
                    {
                        str2 = Xml.GetXMLValue(xml, "firmware/version/upgrade/value", null, null).ToUpper();
                    }
                    return str2;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string FOTAInfoFetch1(string model, string region) => 
            FOTAInfoFetch(model, region, true);

        public static string FOTAInfoFetch2(string model, string region) => 
            FOTAInfoFetch(model, region, false);
    }
}