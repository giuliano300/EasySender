using BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            ViewBag.HeaderPage = "Aggiungi";
            Notifications Notification = new Notifications();
            return View(Notification);
        }

        public async  Task<ActionResult> Edit(int id)
        {
            ViewBag.HeaderPage = "Modifica";
            Notifications n = new Notifications();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Notifications/" + id);
            if (get.IsSuccessStatusCode)
                n = await get.Content.ReadAsAsync<Notifications>();

            return View("New", n);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Save(FormCollection dataForm)
        {
            if (ModelState.IsValid)
            {
                var data = dataForm;
                var name = data["title"];
                var description = data["description"];
                var abilitato = data["enabled"];
                var poste = data["chkPoste"];
                int id = Int32.Parse(data["id"]);


                int notificationType = 1;
                if (poste == null)
                    notificationType = 2;


                Notifications n = new Notifications();

                bool visbile = true;
                if (abilitato == null)
                    visbile = false;


                n.title = name;
                n.description = description;
                n.notificationType = (notificationType)notificationType;
                n.enabled = visbile;
                if (id == 0)
                {
                    await Globals.HttpClientSend("POST", "/api/Notifications/New", n);
                }
                else
                {
                    n.id = id;
                    await Globals.HttpClientSend("POST", "/api/Notifications/Update/" + id, n);
                }

            }
            return RedirectToAction("Index");
        }

    }
}