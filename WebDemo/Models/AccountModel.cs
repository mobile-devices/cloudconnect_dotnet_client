using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;


namespace WebDemo.Models
{
    public class AccountModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public Dictionary<string, string> UrlForwarding { get; set; }

        //[MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        //public string FlatUrlForwarding
        //{
        //    get
        //    {
        //        string result = "";
        //        if (UrlForwarding != null)
        //        {
        //            foreach (String s in UrlForwarding)
        //                result += s + ";";
        //        }
        //        return result;
        //    }
        //    set
        //    {
        //        UrlForwarding = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        //    }
        //}
    }
}
