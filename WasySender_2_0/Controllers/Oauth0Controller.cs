using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Oauth0.Models;

namespace WasySender_2_0.Controllers
{
    public class Oauth0Controller : Controller
    {
        public void authorize()
        {

            var d = Request.Form["id_token"];
        }

    }
}