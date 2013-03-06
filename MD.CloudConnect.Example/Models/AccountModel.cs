using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace CloudConnectReader.Models
{
    public class AccountModel
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string[] UrlForwarding { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public string FlatUrlForwarding
        {
            get
            {
                string result = "";
                if (UrlForwarding != null)
                {
                    foreach (String s in UrlForwarding)
                        result += s + ";";
                }
                return result;
            }
            set
            {
                UrlForwarding = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
