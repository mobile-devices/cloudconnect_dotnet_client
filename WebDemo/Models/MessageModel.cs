using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;

namespace WebDemo.Models
{
    public class MessageModel
    {
        public ObjectId Id { get; set; }
        public ObjectId DeviceID { get; set; }


    }
}