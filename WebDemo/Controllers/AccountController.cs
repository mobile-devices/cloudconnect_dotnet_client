using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDemo.Models.Repository;
using WebDemo.Models;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters;

namespace WebDemo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            @ViewBag.Title = "Accounts";
            return View(RepositoryFactory.Instance.AccountDb.GetAccounts());
        }

        //
        // GET: /Account/Edit/5

        public ActionResult Edit(string id)
        {
            AccountModel account = RepositoryFactory.Instance.AccountDb.GetAccounts().Where(x => x.Id.ToString() == id).FirstOrDefault();
            @ViewBag.Notification = JsonConvert.SerializeObject(account.UrlForwarding, Formatting.None);

            return View(account);
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, AccountModel accountForm, string dataNotification)
        {
            try
            {
                AccountModel account = RepositoryFactory.Instance.AccountDb.GetAccounts().Where(x => x.Id.ToString() == id).FirstOrDefault();
                account.UrlForwarding = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataNotification);
                RepositoryFactory.Instance.AccountDb.Save(account);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
