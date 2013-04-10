using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace WebDemo.Models.Repository
{
    public class AccountDb : RepositoryBase
    {
        private const string ACCOUNT_KEY = "ACCOUNTS_DB";
        private const string ACCOUNT_DB_NAME = "ACCOUNTS";

        public override void RemovedCallback(string k, object v, System.Web.Caching.CacheItemRemovedReason r)
        {
            if (k == ACCOUNT_KEY && r == System.Web.Caching.CacheItemRemovedReason.Expired)
                SaveDataCache(ACCOUNT_KEY, 0, 0, 2, v, autoReload: true);
            base.RemovedCallback(k, v, r);
        }

        public List<AccountModel> GetAccounts()
        {
            List<AccountModel> result = LoadDataCache(ACCOUNT_KEY) as List<AccountModel>;

            if (result == null)
            {
                MongoCollection<AccountModel> accountsDB = Tools.MongoConnector.Instance.DataBaseReadOnly.GetCollection<AccountModel>(ACCOUNT_DB_NAME);
                result = accountsDB.FindAll().ToList();

                SaveDataCache(ACCOUNT_KEY, 0, 0, 2, result, autoReload: true);
            }
            return result;
        }

        public void Save(AccountModel account)
        {
            if (account != null)
            {
                MongoCollection<AccountModel> accountsDB = Tools.MongoConnector.Instance.DataBase.GetCollection<AccountModel>(ACCOUNT_DB_NAME);
                accountsDB.Save(account);

                //Can be Optimize
                HttpRuntime.Cache.Remove(ACCOUNT_KEY);
            }
        }

    }
}