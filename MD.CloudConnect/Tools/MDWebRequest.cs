using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace MD.CloudConnect
{
    public class MDWebRequest
    {
        public string Token { get; set; }
        public string Account { get; set; }
        public string Environment { get; set; }

        private String _data;

        public String Data
        {
            get
            {
                return _data;
            }
        }

        public void Get(string function, string parameter, string per_page = null, string page = null)
        {
            if (String.IsNullOrEmpty(Token) || String.IsNullOrEmpty(Account) || String.IsNullOrEmpty(Environment))
                throw new Exception("You must initialize Token, Account and Environment");

            if (!String.IsNullOrEmpty(per_page))
                parameter += !String.IsNullOrEmpty(parameter) ? "&per_page=" + per_page : "per_page=" + per_page;
            if (!String.IsNullOrEmpty(page))
                parameter += !String.IsNullOrEmpty(parameter) ? "&page=" + page : "page=" + page;

            HttpWebRequest httpRequete = HttpWebRequest.Create(String.Format("http://{0}.{1}.cloudconnect.io/api/v3/{2}{3}"
                , Account
                , Environment
                , function
                , !String.IsNullOrEmpty(parameter) ? "?" + parameter : "")) as HttpWebRequest;
            httpRequete.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Token + ":X"));

            httpRequete.Accept = "application/json";
            httpRequete.CookieContainer = new CookieContainer();

            httpRequete.Method = "GET";

            HttpWebResponse httpReponse = httpRequete.GetResponse() as HttpWebResponse;

            using (StreamReader sr = new StreamReader(httpReponse.GetResponseStream()))
            {
                _data = sr.ReadToEnd();
                sr.Close();
            }
        }

        public void Post(string function, string parameter)
        {

        }
    }
}
