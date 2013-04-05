using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Web;

namespace MD.CloudConnect
{
    public abstract class AbstractApi<T>
    {
        internal MDWebRequest WebRequest { get; set; }

        internal CloudConnect Cloud { get; set; }

        public abstract string Function { get; }

        public virtual List<T> Get(string query = null, string parameter = null, string per_page = null, string page = null)
        {
            string param = "";
            if (!String.IsNullOrEmpty(query))
                param += "q=" + HttpUtility.UrlEncode(query);
            if (!String.IsNullOrEmpty(parameter))
                param += (String.IsNullOrEmpty(param) ? "" : "&") + parameter;
            WebRequest.Get(Function, param, per_page, page);
            return JsonConvert.DeserializeObject<List<T>>(WebRequest.Data);
        }

        public virtual T GetOne(string name = null)
        {
            if (!String.IsNullOrEmpty(name))
            {
                WebRequest.Get(Function + "/" + name, null);
                return JsonConvert.DeserializeObject<T>(WebRequest.Data);
            }
            return default(T);
        }

        public virtual void Post(string json_params)
        {
            WebRequest.Post(Function, json_params);
        }
    }
}
