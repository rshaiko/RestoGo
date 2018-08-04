using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace RestoGo.API
{
    public static class Api
    {
        public static string getJson(string url)
        {
            WebClient client = new WebClient();
            client.Proxy = null;
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string responseFromServer = reader.ReadToEnd();
            return responseFromServer;
        }
    }
}