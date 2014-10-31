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

        private string _cloudUrl;

        public String CloudUrl
        {
            get { return _cloudUrl; }
        }

        private String _data;

        public String Data
        {
            get
            {
                return _data;
            }
        }

        public MDWebRequest(string account, string environment, string token)
        {
            this.Account = account;
            this.Environment = environment;
            this.Token = token;
            switch (environment)
            {
                case "dev":
                    _cloudUrl = String.Format("http://{0}.dev.g8teway.com/api/v3", this.Account);
                    break;
                case "integration":
                    _cloudUrl = String.Format("http://{0}.integration.cloudconnect.io/api/v3", this.Account);
                    break;
                case "production":
                    _cloudUrl = String.Format("http://{0}.cloudconnect.io/api/v3", this.Account);
                    break;
                default:
                    _cloudUrl = String.Format("http://{0}.dev.g8teway.com/api/v3", this.Account);
                    break;
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

            HttpWebRequest request = HttpWebRequest.Create(String.Format("{0}/{1}{2}"
                , CloudUrl
                , function
                , !String.IsNullOrEmpty(parameter) ? "?" + parameter : "")) as HttpWebRequest;
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Token + ":X"));

            request.Accept = "application/json";
            request.CookieContainer = new CookieContainer();
            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                _data = sr.ReadToEnd();
                sr.Close();
            }
        }

        public void Post(string function, string parameter)
        {
            if (String.IsNullOrEmpty(Token) || String.IsNullOrEmpty(Account) || String.IsNullOrEmpty(Environment))
                throw new Exception("You must initialize Token, Account and Environment");
            if (String.IsNullOrEmpty(parameter))
                throw new ArgumentNullException("Content message for a post request can not be null or blank");

            HttpWebRequest httpRequete = HttpWebRequest.Create(String.Format("{0}/{1}"
                , CloudUrl
                , function)) as HttpWebRequest;
            httpRequete.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Token + ":X"));

            httpRequete.Accept = "application/json";
            httpRequete.ContentType = "application/json";
            httpRequete.CookieContainer = new CookieContainer();

            httpRequete.Method = "POST";

            using (StreamWriter sw = new StreamWriter(httpRequete.GetRequestStream()))
            {
                sw.Write(parameter);
            }

            HttpWebResponse httpReponse = httpRequete.GetResponse() as HttpWebResponse;

            using (StreamReader sr = new StreamReader(httpReponse.GetResponseStream()))
            {
                _data = sr.ReadToEnd();
                sr.Close();
            }
        }
    }
}
