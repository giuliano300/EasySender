using BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace BackOffice.Controllers
{
    public class login
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(FormCollection dataForm)
        {
            login login = new login();
            login.email = dataForm[1];
            login.password = dataForm[2];

            if(login.email == Globals.staticUsername && login.password == Globals.staticPwd)
            {
                setlogin();
                return Redirect("/Home");
            }

            ViewBag.errormessage = "username o password errata!";
            return View("Index");

        }

        public void setlogin()
        {
            Session["connessoH2H"] = true;
        }


        public ActionResult LogOff()
        {
            Session["connessoH2H"] = false;
            return RedirectToAction("Index");
        }
    }
}