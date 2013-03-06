using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace CloudConnectReader.Tools
{
    public class MongoConnector
    {
        private MongoDatabase _dataBase;

        #region singleton
        static readonly MongoConnector _instance = new MongoConnector();
        public static MongoConnector Instance
        {
            get
            {
                lock (_instance)
                {
                    return _instance;
                }
            }
        }

        static MongoConnector()
        {

        }
        #endregion

        public MongoDatabase DataBase
        {
            get
            {
                if (_dataBase != null && _dataBase.Server != null && _dataBase.Server.State == MongoServerState.Connected)
                    return _dataBase;
                else
                {
                    var connectionString = "mongodb://localhost";
                    var client = new MongoClient(connectionString);
                    var server = client.GetServer();
                    _dataBase = server.GetDatabase("CloudConnectData");

                    return _dataBase;
                }
            }
        }

        public MongoDatabase DataBaseReadOnly
        {
            get
            {
                var connectionString = "mongodb://localhost";
                var client = new MongoClient(connectionString);
                var server = client.GetServer();

                return server.GetDatabase("CloudConnectData");

            }
        }
    }

}