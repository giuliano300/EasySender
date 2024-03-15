using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WasySender_2_0.DataModel;
using WasySender_2_0.Models;
using WasySender_2_0.ViewModel;

namespace WasySender_2_0.Controllers
{
    public class UserController : Controller
    {

        public async Task<ActionResult> Utenti()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);
    
            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            //USERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Users>();
            get = await Globals.HttpClientSend("GET", "Users?parentId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Users>>();

            string description = "Visualizzazione utenti";
            int logType = (int)LogType.crudUsers;
            await Globals.SetLogs(logType, u.id, description);

            return View(s);
        }

        public async Task<ActionResult> Mittenti()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            //SENDERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Sender>();
            get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Sender>>();

            string description = "Visualizzazione mittenti";
            int logType = (int)LogType.crudSender;
            await Globals.SetLogs(logType, u.id, description);

            return View(s);
        }

        [HttpGet]
        public async Task<string> GetMittenti()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            //SENDERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Sender>();
            get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Sender>>();

            return new JavaScriptSerializer().Serialize(s);
        }

        [HttpGet]
        public async Task<string> GetDestinatari()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            //SENDERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<NamesLists>();
            get = await Globals.HttpClientSend("GET", "NamesLists/TemporaryList?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<NamesLists>>();

            s = s
                .GroupBy(a => a.address)
                .Select(g => g.First())
                .GroupBy(a => a.businessName)
                .Select(f => f.First())
                .OrderBy(a => a.businessName)
                .ToList();

            return new JavaScriptSerializer().Serialize(s);
        }

        [HttpGet]
        public async Task<string> GetMittente(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            //SENDER
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new Sender();
            get = await Globals.HttpClientSend("GET", "SendersUsers/" + id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<Sender>();

            return new JavaScriptSerializer().Serialize(s);
        }

        [HttpGet]
        public async Task<string> GetSenders()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            //SENDER
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<string>();
            get = await Globals.HttpClientSend("GET", "SendersUsers/GetSenders?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<string>>();

            return new JavaScriptSerializer().Serialize(s);
        }


        [HttpGet]
        public async Task<string> GetUsers()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            //USERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Users>();
            get = await Globals.HttpClientSend("GET", "Users?parentId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Users>>();

            return new JavaScriptSerializer().Serialize(s);
        }


        [HttpGet]
        public async Task<string> GetDestinatario(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            //SENDER
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new NamesLists();
            get = await Globals.HttpClientSend("GET", "NamesLists/Item?id=" + id + "&guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<NamesLists>();

            return new JavaScriptSerializer().Serialize(s);
        }

        public async Task<ActionResult> Destinatari()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            var l = new List<GetLists>();
            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "Lists?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });

            l = await get.Content.ReadAsAsync<List<GetLists>>();
            return View(l);
        }

        public ActionResult AddDestinatari()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            ViewBag.userId = u.id;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.areaTestUser = u.areaTestUser;

            return View();
        }

        public ActionResult AddMittenti()
        {
            var s = new Sender();
            return View(s);
        }

        public async Task<ActionResult> ModMittente(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            HttpResponseMessage get = new HttpResponseMessage();
            var s = new Sender();
            get = await Globals.HttpClientSend("GET", "SendersUsers/" + id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<Sender>();

            return View("AddMittenti",s);
        }

        public string ValidateSender(FormCollection fc)
        {
            Sender s = new Sender()
            {
                businessName = fc["ragsoc"],
                name = "",
                surname = "",
                dug = "",
                address = fc["indirizzo"],
                houseNumber = "",
                cap = fc["cap"],
                city = fc["citta"],
                province = fc["provincia"],
                state = fc["stato"],
                complementNames = fc["complementNames"],
                complementAddress = fc["complementAddress"]
            };

            var c = new ControlloMittente();
            c = CheckSender.verificaMittente(s);

            return new JavaScriptSerializer().Serialize(c);
        }

        public async Task<int> SaveSender(FormCollection fc)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);


            Sender s = new Sender()
            {
                businessName = fc["ragsoc"],
                name = "",
                surname = "",
                dug = "",
                address = fc["indirizzo"],
                houseNumber = "",
                cap = fc["cap"],
                city = fc["citta"],
                province = fc["provincia"],
                state = fc["stato"],
                complementNames = fc["complementNames"],
                complementAddress = fc["complementAddress"],
                telefono = fc["telefono"],
                email = fc["email"],
                userId = u.id,
                temporary = false,
                id = Convert.ToInt32(fc["id"])
            };

            int id = Convert.ToInt32(fc["id"]);
            string description = "Modifica mittente id " + id;

            HttpResponseMessage get = null;
            if (id == 0)
            {
                get = await Globals.HttpClientSend("POST", "SendersUsers/New", u.areaTestUser, s);
                if (get.IsSuccessStatusCode)
                    id = await get.Content.ReadAsAsync<int>();

                description  = "Aggiunta nuovo mittente id " + id;
            }
            else
                get = await Globals.HttpClientSend("POST", "SendersUsers/Update/" + Convert.ToInt32(fc["id"]), u.areaTestUser, s);


            int logType = (int)LogType.crudSender;
            await Globals.SetLogs(logType, u.id, description);


            return id;
        }

        public string ValidateName(FormCollection fc)
        {
            var comuni = Globals.GetComuniList();

            NamesLists s = new NamesLists()
            {
                businessName = fc["ragsoc"],
                name = fc["nome"],
                surname = fc["cognome"],
                complementNames = fc["complementNames"],
                address = fc["indirizzo"],
                complementAddress = fc["complementAddress"],
                fiscalCode = fc["fiscalCode"],
                cap = fc["cap"],
                city = fc["citta"],
                province = fc["provincia"],
                state = fc["stato"]
            };

            var comune = comuni.Where(a => a.cap == fc["cap"]);

            var c = new ControlloDestinatario();
            c = CheckRecipient.verificaDestinatario(s, comune);

            return new JavaScriptSerializer().Serialize(c);
        }

        public async Task<int> SaveName(FormCollection fc)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            NamesLists s = new NamesLists()
            {
                businessName = fc["ragsoc"],
                name = fc["nome"],
                surname = fc["cognome"],
                complementNames = fc["complementNames"],
                address = fc["indirizzo"],
                complementAddress = fc["complementAddress"],
                fiscalCode = fc["fiscalCode"],
                cap = fc["cap"],
                city = fc["citta"],
                province = fc["provincia"],
                state = fc["stato"],
                listId = Convert.ToInt32(fc["listId"]),
                id = Convert.ToInt32(fc["id"])
            };

            int id = Convert.ToInt32(fc["id"]);
            HttpResponseMessage get = null;
            if (id == 0)
            {
                get = await Globals.HttpClientSend("POST", "/api/NamesLists/New", u.areaTestUser, s);
                if (get.IsSuccessStatusCode)
                    id = await get.Content.ReadAsAsync<int>();
            }
            else
                get = await Globals.HttpClientSend("POST", "/api/NamesLists/Update/" + Convert.ToInt32(fc["id"]), u.areaTestUser, s);


            return id;
        }

        public async Task<bool> DeleteSender(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = null;
            get = await Globals.HttpClientSend("GET", "/SendersUsers/Delete?id=" + id, u.areaTestUser);
  
            
            string description = "Eliminazione mittente id " + id;
            int logType = (int)LogType.crudSender;
            await Globals.SetLogs(logType, u.id, description);

            return true;
        }

        public async Task<bool> DeleteNames(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = null;
            get = await Globals.HttpClientSend("GET", "/api/NamesLists/Delete?id=" + id, u.areaTestUser);
            return true;
        }

        public async Task<ActionResult> DeleteUser(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = null;
            get = await Globals.HttpClientSend("GET", "/api/Users/Delete/" + id, u.areaTestUser);


            string description = "Eliminazione utente id " + id;

            int logType = (int)LogType.crudUsers;
            await Globals.SetLogs(logType, u.id, description);

            return Redirect("/User/Utenti");
        }

        public async Task<bool> DeleteList(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = null;
            get = await Globals.HttpClientSend("GET", "/api/Lists/Delete?id=" + id, u.areaTestUser);
            return true;
        }

        public async Task<ActionResult> AddUtenti()
        {
            var us = new UserViewModel();
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            //SENDERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Sender>();
            get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Sender>>();

            us.senders = s;

            return View(us);
        }

        public async Task<ActionResult> ModUtenti(int id)
        {
            var us = new UserViewModel();
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);
 
            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            //SENDERS
            HttpResponseMessage get = new HttpResponseMessage();
            var s = new List<Sender>();
            get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<List<Sender>>();

            var user = new Users();
            get = await Globals.HttpClientSend("GET", "Users/" + id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                user = await get.Content.ReadAsAsync<Users>();

            string[] selected = null;
            if (user.sendersId != null)
                selected = user.sendersId.Split(',');

            us.senders = s;
            us.user = user;
            us.selectedSender = selected;

            return View("AddUtenti", us);
        }

        public ActionResult AddDestinatario(int id)
        {
            ViewBag.listId = id;
            var d = new NamesLists();
            return View(d);
        }

        public async Task<ActionResult> ModDestinatario(int id, int id2)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);
            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            HttpResponseMessage get = new HttpResponseMessage();
            var d = new NamesLists();
            get = await Globals.HttpClientSend("GET", "api/NamesLists/Item?id=" + id2 + "&guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
               d = await get.Content.ReadAsAsync<NamesLists>();
            ViewBag.listId = id;

            return View("AddDestinatario",d);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> UploadList()
        {
            var r = new UploadListResponse();
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var file = Request.Files[0];

            if (file != null && file.ContentLength > 0)
            {
                var ex = Path.GetExtension(file.FileName);
                if (ex == ".csv")
                {
                    var name = DateTime.Now.Ticks + ex;
                    var directory = Server.MapPath("/Upload/FileCsv/");
                    var dbDirectory = Globals.staticUrl + "Upload/FileCsv/";
                    file.SaveAs(Path.Combine(directory + name));

                    UploadListResponse res = await Globals.ReadCsvShowResults(dbDirectory + name, u, Session.SessionID, "");

                    Session["UploadResponse"] = res;
                    return new JavaScriptSerializer().Serialize(res);

                }
            }

            return new JavaScriptSerializer().Serialize(r);
        }

        public async Task<ActionResult> SalvaLista(FormCollection dataForm)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var l = new Lists();
            l.name = dataForm["nomeLista"];
            l.description = "Caricamento file in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;

            int id = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "/api/Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();

            if (id == 0)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });

            UploadListResponse n = (UploadListResponse)Session["UploadResponse"];
            GetRecipentLists ln = new GetRecipentLists();
            n.NamesLists.ToList().ForEach(a => a.listId = id);
            ln.recipient = n.NamesLists.Where(a => a.valid == true).ToList();

            ln.bulletin = null;

            get = await Globals.HttpClientSend("POST", "/api/NamesLists/NewMultipleWithBulletin", u.areaTestUser, ln);
            if (!get.IsSuccessStatusCode)
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });
            }

            Session["UploadResponse"] = null;

            return Redirect("/User/Destinatari");
        }

        public async Task<ActionResult> SalvaListaVuota(FormCollection dataForm)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var l = new Lists();
            l.name = dataForm["nomeLista"];
            l.description = "Caricamento file in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;

            int id = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "/api/Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();

            if (id == 0)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });

            return Redirect("/User/Destinatari");
        }

        public async Task<ActionResult> Details(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            var l = new GetLists();
            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "api/Lists/" + id + "?guidUser=" + u.guidUser, u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });

            l = await get.Content.ReadAsAsync<GetLists>();
            return View(l);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string SearchCity(FormCollection fc)
        {
            var cap = fc["cap"];
            var c = Globals.GetComuneFromCap(cap);

            return new JavaScriptSerializer().Serialize(c);
        }

        public async Task<ActionResult> SalvaUser(FormCollection fc)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var h = false;
            if (fc["hidePrice"] == "on")
                h = true;

            var uc = new Users()
            {
                usernamePoste = u.usernamePoste,
                pwdPoste = u.pwdPoste,
                pwd = fc["pwd"],
                porpertyId = u.porpertyId,
                name = fc["nome"],
                lastName = fc["cognome"],
                userType = Convert.ToInt32(fc["userType"]),
                username = fc["email"],
                email = fc["email"],
                hidePrice = h,
                baseUrl = u.baseUrl,
                guidUser = u.guidUser,
                address = u.address,
                cap = u.cap,
                city = fc["citta"],
                province = u.province,
                mobile = fc["telefono"],
                parentId = u.id,
                sendersId = fc["sender"],
                areaTestUser = u.areaTestUser,
                demoUser = u.demoUser,
                usernamePosteAreaTest = u.usernamePosteAreaTest,
                pwdPosteAreaTest = u.pwdPosteAreaTest,
                userPriority = u.userPriority,
                changePwd = true,
                mol = u.mol,
                col = u.col,
                attivoPA = u.attivoPA,
                downloadFile = u.downloadFile,
                CodiceContrattoCOL = u.CodiceContrattoCOL,
                CodiceContrattoMOL = u.CodiceContrattoMOL,
                businessName = u.businessName,
                conciliazioneBollettini = u.conciliazioneBollettini,
                abilitato = true
            };

            var j = JsonConvert.SerializeObject(uc);

            int id = Convert.ToInt32(fc["id"]);
            string description = "Modifica utente id " + id;
            HttpResponseMessage get = null;
            if (id == 0)
            {
                get = await Globals.HttpClientSend("POST", "/api/Users/New", u.areaTestUser, uc);
                if (get.IsSuccessStatusCode)
                    id = await get.Content.ReadAsAsync<int>();
               
                description = "Inserimento nuovo utente id " + id;
            }
            else {
                uc.id = Convert.ToInt32(fc["id"]);
                get = await Globals.HttpClientSend("POST", "/api/Users/Update/" + Convert.ToInt32(fc["id"]), u.areaTestUser, uc);
            }


            int logType = (int)LogType.crudUsers;
            await Globals.SetLogs(logType, u.id, description);


            return Redirect("/User/Utenti");
        }

        [HttpPost]
        [ValidateInput(false)]
        public string GetNames(FormCollection fc)
        {
            int index = Convert.ToInt32(fc["index"]);

            UploadListResponse n = (UploadListResponse)Session["UploadResponse"];

            var t = n.NamesLists.Select((c, i) => new { NamesLists = c, Index = i }).Where(x => x.Index == index);
            var ne = t.FirstOrDefault();
            var name = ne.NamesLists;


            return new JavaScriptSerializer().Serialize(name);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string SaveNameList(FormCollection fc)
        {
            int index = Convert.ToInt32(fc["index"]);

            UploadListResponse n = (UploadListResponse)Session["UploadResponse"];
            var t = n.NamesLists.Select((c, i) => new { NamesLists = c, Index = i }).Where(x => x.Index == index);
            var ne = t.FirstOrDefault();
            var name = ne.NamesLists;

            NamesLists s = new NamesLists()
            {
                businessName = fc["ragsoc"],
                name = fc["nome"],
                surname = fc["cognome"],
                dug = fc["dug"],
                address = fc["indirizzo"],
                houseNumber = fc["numeroCivico"],
                cap = fc["cap"],
                city = fc["citta"],
                province = fc["provincia"],
                state = fc["stato"],
                listId = 0,
                id = 0,
                valid = true
            };

            List<NamesLists> ls = new List<NamesLists>();

            var l = n.NamesLists.ToList();

            for (var i = 0; i < l.Count(); i++){
                if (i == index)
                    ls.Add(s);
                else
                    ls.Add(l[i]);
            }

            UploadListResponse nn = new UploadListResponse()
            {
                success = true,
                errorMessage = "",
                listId = 0,
                numberOfNames = n.numberOfNames,
                errors = (!name.valid ? n.errors - 1 : n.errors),
                NamesLists = ls,
                Bulletins = null
            };

            Session["UploadResponse"] = nn;
            return new JavaScriptSerializer().Serialize(nn);
        }

        public async Task<decimal> GetValidateNames()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var d = new ValidateNamesProgress();
            decimal percentage = 0;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/TemporaryValidateTable?userId=" + u.id + "&sessionId=" + Session.SessionID, false);
            if (get.IsSuccessStatusCode)
            {
                var g = await get.Content.ReadAsAsync<List<TemporaryValidateTable>>();
                if (g.Count() > 0)
                {
                    d.number = g.Count();
                    d.total = g.FirstOrDefault().totalNames;
                    decimal perc = Decimal.Divide(d.number, d.total);
                    d.percentage = Math.Round(perc * 100, 0);

                    percentage = d.percentage;
                }
            }

            return percentage;

        }


        public string GetBulletinCustomerCode(string anno, string cap)
        {
            return Globals.GetCodiceClienteBollettino(anno, cap);
        }
    }
}