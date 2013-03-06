using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CloudConnectReader.Models.Repository;
using CloudConnectReader.Models;

namespace CloudConnectReader.Controllers
{
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
            return View(account);
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(string id, AccountModel accountForm)
        {
            try
            {
                AccountModel account = RepositoryFactory.Instance.AccountDb.GetAccounts().Where(x => x.Id.ToString() == id).FirstOrDefault();
                account.UrlForwarding = accountForm.UrlForwarding;

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
