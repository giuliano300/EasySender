using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WasySender_2_0.Models;

namespace WasySender_2_0.Controllers
{
    public class LoghiController : Controller
    {
        // GET: Loghi
        public async Task<ActionResult> Index()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            var add = "";
            if (u.parentId > 0)
                add = "&parentId=" + u.parentId;

            //USERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Loghi>();
            get = await Globals.HttpClientSend("GET", "Loghi?userId=" + u.id + add, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Loghi>>();

            int logType = (int)LogType.visLoghi;
            string description = "Visualizzazione loghi";

            await Globals.SetLogs(logType, u.id, description);

            return View(s);
        }
        public async Task<ActionResult> AddLogo()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            int logType = (int)LogType.addLogo;
            string description = "Aggiunta logo";

            await Globals.SetLogs(logType, u.id, description);

            return View();
        }
        public async Task<bool> Delete(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Loghi/Delete?id=" + id, u.areaTestUser);

            return true;
        }
        public async Task<ActionResult> SaveLogo(FormCollection fc)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });


            var logo = new Loghi()
            {
                name = fc["name"],
                userId = u.id
            };

            if (u.parentId > 0)
                logo.parentUserId = u.parentId;

            var file = Request.Files[0];

            if (file != null && file.ContentLength > 0)
            {
                var ex = Path.GetExtension(file.FileName);
                var name = DateTime.Now.Ticks + ex;
                var directory = Server.MapPath("/Upload/Loghi/");
                var dbDirectory = Globals.staticUrl + "Upload/Loghi/";
                file.SaveAs(Path.Combine(directory + name));

                logo.logo = name;
            }

            HttpResponseMessage get = await Globals.HttpClientSend("POST", "Loghi/New", u.areaTestUser, logo);


            return Redirect("/Loghi/");
        }
    }
}