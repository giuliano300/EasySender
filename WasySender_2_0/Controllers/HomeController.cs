using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> SsoLoginRandstad(FormCollection f)
        {

            var token = f["id_token"];
            var j = Globals.GetJwtUser(token);

            var u = new GetUserAndPermissions();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/Users/GetUserByUsername?username=" + j.nickname + "&prefix=" + Globals.prefixRandstad, false);
            if (get.IsSuccessStatusCode)
                u = await get.Content.ReadAsAsync<GetUserAndPermissions>();

            if (u.user == null)
                return Redirect("/Home/Error");


            if (Request.Cookies["login"] != null)
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["permits"] != null)
                Response.Cookies["permits"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["allPermits"] != null)
                Response.Cookies["allPermits"].Expires = DateTime.Now.AddDays(-1);


            //LOGIN
            //RICAVARE DATA DI SCADENZA
            var json = JsonConvert.SerializeObject(u.user);
            var permits = JsonConvert.SerializeObject(u.permits);
            Response.Cookies["login"].Value = json;
            Response.Cookies["login"].Expires = DateTime.Now.AddMilliseconds(j.exp);

            Response.Cookies["permits"].Value = permits;
            Response.Cookies["permits"].Expires = DateTime.Now.AddMilliseconds(j.exp);

            Response.Cookies["allPermits"].Value = u.all.ToString();
            Response.Cookies["allPermits"].Expires = DateTime.Now.AddMilliseconds(j.exp);


            return Redirect("/");
        }

        public void SsoLogout()
        {
            if (Request.Cookies["login"] != null)
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["permits"] != null)
                Response.Cookies["permits"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["allPermits"] != null)
                Response.Cookies["allPermits"].Expires = DateTime.Now.AddDays(-1);

        }

        public ActionResult Sondaggio()
        {
            return View();
        }

        public ActionResult PwdRecovery()
        {
            return View();
        }

        public async Task<ActionResult> MassiveImport()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.ProductTypeName = "MASSIVA";
            ViewBag.ProductTypeId = 0;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.userId = u.id;

            var senders = new List<Sender>();
            var loghi = new List<Loghi>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                senders = await get.Content.ReadAsAsync<List<Sender>>();


            get = await Globals.HttpClientSend("GET", "Loghi?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                loghi = await get.Content.ReadAsAsync<List<Loghi>>();

            var loghiSenders = new LoghiSenders()
            {
                senders = senders,
                loghi = loghi
            };

            return View(loghiSenders);
        }

        public async Task<ActionResult> MassiveImportRol()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.ProductTypeName = "MASSIVA";
            ViewBag.ProductTypeId = 0;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.userId = u.id;

            var senders = new List<Sender>();
            var loghi = new List<Loghi>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                senders = await get.Content.ReadAsAsync<List<Sender>>();


            get = await Globals.HttpClientSend("GET", "Loghi?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                loghi = await get.Content.ReadAsAsync<List<Loghi>>();

            var loghiSenders = new LoghiSenders()
            {
                senders = senders,
                loghi = loghi
            };

            return View(loghiSenders);
        }

        public async Task<string> ImportMassiveLettere(FormCollection f)
        {
            var sender = f["Sender"];
            var logo = f["logo"];
            var fronteRetro = f["FronteRetro"];
            var tipoStampa = f["TipoStampa"];
            var tipoLettera = f["TipoLettera"];

            var ff = false;
            if (fronteRetro == "1")
                ff = true;

            var ts = true;
            if (tipoStampa == "0")
                ts = false;

            var sessionId = f["sessionId"];
            var dbDirectory = Globals.staticUrl + "/Public/MassiveImport/Csv/";
            var fileDirectory = Server.MapPath("/Public/MassiveImport/Zip/");
            var name = f["fileNameCsv"];


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;

            
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            Response.Cookies["UploadResponse"].Value = "";
            Response.Cookies["UploadFile"].Value = "";


            UploadListResponse res = null;

            res = await Globals.ReadCsvShowResultsMassive(dbDirectory + name, u, sessionId, logo, fileDirectory, ff);
            try
            {
                Response.Cookies["UploadResponse"].Value = serializer.Serialize(res);
                Response.Cookies["UploadFile"].Value = dbDirectory + name;
            }
            catch (Exception e) 
            { 
            
            }

            return serializer.Serialize(res);

        }

        public async Task<string> ImportMassiveRaccomandate(FormCollection f)
        {
            var sender = f["Sender"];
            var logo = f["logo"];
            var fronteRetro = f["FronteRetro"];
            var tipoStampa = f["TipoStampa"];
            var RicevutaRitorno = f["RicevutaRitorno"];

            var ff = false;
            if (fronteRetro == "1")
                ff = true;

            var ts = true;
            if (tipoStampa == "0")
                ts = false;

            var sessionId = f["sessionId"];
            var dbDirectory = Globals.staticUrl + "/Public/MassiveImport/Csv/";
            var fileDirectory = Server.MapPath("/Public/MassiveImport/Zip/");
            var name = f["fileNameCsv"];

            int SenderAR = 0;

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (f["RicevutaRitorno"] == "1")
            {
                //ESISTE LA RICEVUTA DI RITORNO
                //INSERISCO IL DESTINATARIO AR
                var NominativoMittenteAR = f["nominativoSenderAR"];
                var CompletamentoNominativoMittenteAR = f["complementoNominativoSenderAR"];
                var IndirizzoMittenteAR = f["indirizzoSenderAR"];
                var CompletamentoIndirizzoMittenteAR = f["completamentoIndirizzoSenderAR"];
                var CapMittenteAR = f["capSenderAR"];
                var CittaMittenteAR = f["cittaSenderAR"];
                var ProvinciaMittenteAR = f["provinciaSenderAR"];
                var StatoMittenteAR = f["statoSenderAR"];

                var sAR = new Sender()
                {
                    businessName = NominativoMittenteAR,
                    complementNames = CompletamentoNominativoMittenteAR,
                    complementAddress = CompletamentoIndirizzoMittenteAR,
                    address = IndirizzoMittenteAR,
                    cap = CapMittenteAR,
                    city = CittaMittenteAR,
                    province = ProvinciaMittenteAR,
                    state = StatoMittenteAR
                };

                var cMAR = new ControlloMittente();
                cMAR = CheckSender.verificaMittente(sAR);


                HttpResponseMessage get = new HttpResponseMessage();

                //CREAZIONE MITTENTE AR
                sAR.temporary = true;
                sAR.userId = u.id;
                get = await Globals.HttpClientSend("POST", "SendersUsers/New", u.areaTestUser, sAR);
                if (get.IsSuccessStatusCode)
                    SenderAR = await get.Content.ReadAsAsync<int>();

            }


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = Int32.MaxValue;


            Response.Cookies["UploadResponse"].Value = "";
            Response.Cookies["UploadFile"].Value = "";
            Response.Cookies["SenderAR"].Value = "";


            UploadListResponse res = null;

            res = await Globals.ReadCsvShowResultsMassive(dbDirectory + name, u, sessionId, logo, fileDirectory, ff);
            res.senderArId = SenderAR;
            try
            {
                Response.Cookies["UploadResponse"].Value = serializer.Serialize(res);
                Response.Cookies["SenderAR"].Value = SenderAR.ToString();
                Response.Cookies["UploadFile"].Value = dbDirectory + name;
            }
            catch (Exception e)
            {

            }

            return serializer.Serialize(res);

        }

        public async Task<string> CreateErrorFileMassive(FormCollection l)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var res = JsonConvert.DeserializeObject<UploadListResponse>(l["data"]);

            var errorList = res.NamesLists.Where(a => a.valid == false);

            string csv = "RagioneSociale;Nome;Cognome;CAP;Citta;Provincia;Stato;Indirizzo;CompletamentoIndirizzo;CompletamentoNominativo;NomeFile;CodiceFiscale;Telefono;Errore\n";
            foreach (var e in errorList)
            {
                csv += e.businessName + ";" + e.name + ";" + e.surname + ";" + e.cap + ";" + e.city + ";" + e.province + ";" + e.state + ";" + e.address + ";" + e.complementAddress + ";" + e.complementNames + ";" + e.fileName + ";" + e.fiscalCode + ";" + e.mobile + ";" + e.errorMessage + "\n";
            }

            var name = "Errors-" + DateTime.Now.Ticks.ToString() + ".csv";

            try { 
                System.IO.File.WriteAllText(Server.MapPath("/Public/" + name), csv.ToString());
            }
            catch(Exception e)
            {
                var rr = e;
            };

            return "/Public/" + name;
        }

        public async Task<int> CreateList()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var l = new Lists();
            l.name = "Lista caricamento";
            l.description = "Caricamento file in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;

            int id = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();

            if (id == 0)
                return 0;

            return id;
        }

        public async Task<string> CreateOperation(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            int operationTypes = (int)operationType.LOL;
            if (u.col)
                operationTypes = (int)operationType.COL;

            var op = new Operations()
            {
                areaTestOperation = u.areaTestUser,
                userId = u.id,
                userParentId = u.parentId,
                date = DateTime.Now,
                operationType = operationTypes,
                demoOperation = false,
                complete = true,
                operationPriority = u.userPriority,
                name = "Operazione Massiva del " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"),
            };

            int operationId = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "Operations/New", u.areaTestUser, op);
            if (get.IsSuccessStatusCode)
                operationId = await get.Content.ReadAsAsync<int>();

            if (operationId == 0)
                return null;

            var s = new SenderOperations();
            get = await Globals.HttpClientSend("GET", "SendersUsers/" + id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<SenderOperations>();

            s.operationId = operationId;

            int senderId = 0;
            get = await Globals.HttpClientSend("POST", "Sender/New", u.areaTestUser, s);
            if (get.IsSuccessStatusCode)
                senderId = await get.Content.ReadAsAsync<int>();

            if (senderId == 0)
                return null;

            var li = new List<int>();
            li.Add(operationId);
            li.Add(senderId);

            return JsonConvert.SerializeObject(li);
        }

        public async Task<string> CreateOperationRol(int id, int arId = 0)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            int operationTypes = (int)operationType.ROL;
            if (u.mol)
                operationTypes = (int)operationType.MOL;

            var op = new Operations()
            {
                areaTestOperation = u.areaTestUser,
                userId = u.id,
                userParentId = u.parentId,
                date = DateTime.Now,
                operationType = operationTypes,
                demoOperation = false,
                complete = true,
                operationPriority = u.userPriority,
                name = "Operazione Massiva del " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"),
            };

            int operationId = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "Operations/New", u.areaTestUser, op);
            if (get.IsSuccessStatusCode)
                operationId = await get.Content.ReadAsAsync<int>();

            if (operationId == 0)
                return null;

            var s = new SenderOperations();
            get = await Globals.HttpClientSend("GET", "SendersUsers/" + id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                s = await get.Content.ReadAsAsync<SenderOperations>();

            s.operationId = operationId;

            int senderId = 0;
            get = await Globals.HttpClientSend("POST", "Sender/New", u.areaTestUser, s);
            if (get.IsSuccessStatusCode)
                senderId = await get.Content.ReadAsAsync<int>();

            int senderArId = 0;

            if(arId > 0) {
                var sAr = new SenderOperations();
                get = await Globals.HttpClientSend("GET", "SendersUsers/" + arId, u.areaTestUser);
                if (get.IsSuccessStatusCode)
                    sAr = await get.Content.ReadAsAsync<SenderOperations>();

                sAr.AR = true;
                sAr.operationId = operationId;

                get = await Globals.HttpClientSend("POST", "Sender/New", u.areaTestUser, sAr);
                if (get.IsSuccessStatusCode)
                    senderArId = await get.Content.ReadAsAsync<int>();
            }

            if (senderId == 0)
                return null;

            if (senderArId == 0)
                return null;

            var li = new List<int>();
            li.Add(operationId);
            li.Add(senderId);
            li.Add(senderArId);

            return JsonConvert.SerializeObject(li);
        }

        public async Task<int> CreateNamesRol(FormCollection d)
        {

            int operationId = Convert.ToInt32(d["operationId"]);
            int senderId = Convert.ToInt32(d["senderId"]);
            var tsc = d["tsc"];
            var frc = d["frc"];
            var ricevutaRitorno = d["RicevutaRitorno"];

            var rr = 1;
            if (ricevutaRitorno == "0")
                rr = 0;

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            Names l = new Names()
            {
                businessName = d["d[businessName]"],
                name = d["d[name]"],
                surname = d["d[surname]"],
                dug = d["d[dug]"],
                address = d["d[address]"],
                houseNumber = d["d[houseNumber]"],
                cap = d["d[cap]"],
                city = d["d[city]"],
                province = d["d[province]"],
                state = d["d[state]"],
                fileName = d["d[fileName]"],
                complementNames = d["d[complementNames]"],
                complementAddress = d["d[complementAddress]"],
                insertDate = DateTime.Now,
                price = 0,
                vatPrice = 0,
                totalPrice = 0,
                fiscalCode = d["d[fiscalCode]"],
                mobile = d["d[mobile]"],
                tipoLettera = "",
                tipoStampa = false,
                fronteRetro = false,
                ricevutaRitorno = false,
                valid = true,
                currentState = 0,
                stato = "",
                requestId = null,
                orderId = null,
                codice = null,
                presaInCaricoDate = DateTime.Now,
                consegnatoDate = DateTime.Now,
                pathRecoveryFile = d["d[pathRecoveryFile]"],
                NREA = d["d[NREA]"],
                codiceDocumento = 0,
                tipoDocumento = 0,
                pathGEDUrl = d["d[pathGEDUrl]"],
                product = d["d[product]"],
                shipmentDate = null,
                weight = null,
                height = null,
                length = null,
                width = null,
                contentText = d["d[contentText]"],
                pathUrl = d["d[pathUrl]"],
                additionalServices = d["d[additionalServices]"],
                senderFromContract = false,
                sms = false,
                testoSms = d["d[testoSms]"],
                logo = d["d[logo]"],
                id = 0,
            };

            var j = JsonConvert.SerializeObject(l);

            int id = 0;
            var url = "Rol/NewOne?operationId=" + operationId + "&tsc=" + tsc + "&frc=" + frc + "&frm=false&userId=" + u.id + "&ricevutaRitorno=" + rr;

            HttpResponseMessage get = await Globals.HttpClientSend("POST", url, u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();

            if (id == 0)
                return 0;

            return id;
        }


        public async Task<int> CreateNames(FormCollection d)
        {

            int operationId = Convert.ToInt32(d["operationId"]);
            int senderId = Convert.ToInt32(d["senderId"]);
            var tsc = d["tsc"];
            var frc = d["frc"];
            var TipoLettera = d["TipoLettera"];

            //bool tipoStampa = false;
            //if (tsc == "0")
            //    tipoStampa = true;

            //bool fronteRetro = false;
            //if (frc == "1")
            //    fronteRetro = true;

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            Names l = new Names() {
                businessName = d["d[businessName]"],
                name = d["d[name]"],
                surname = d["d[surname]"],
                dug = d["d[dug]"],
                address = d["d[address]"],
                houseNumber = d["d[houseNumber]"],
                cap = d["d[cap]"],
                city = d["d[city]"],
                province = d["d[province]"],
                state = d["d[state]"],
                fileName = d["d[fileName]"],
                complementNames = d["d[complementNames]"],
                complementAddress = d["d[complementAddress]"],
                insertDate = DateTime.Now,
                price = 0,
                vatPrice = 0,
                totalPrice = 0,
                fiscalCode = d["d[fiscalCode]"],
                mobile = d["d[mobile]"],
                tipoLettera = "",
                tipoStampa = false,
                fronteRetro = false,
                ricevutaRitorno = false,
                valid = true,
                currentState = 0,
                stato = "",
                requestId = null,
                orderId = null,
                codice = null,
                presaInCaricoDate = DateTime.Now,
                consegnatoDate = DateTime.Now,
                pathRecoveryFile = d["d[pathRecoveryFile]"],
                NREA = d["d[NREA]"],
                codiceDocumento = 0,
                tipoDocumento = 0,
                pathGEDUrl = d["d[pathGEDUrl]"],
                product = d["d[product]"],
                shipmentDate = null,
                weight = null,
                height = null,
                length = null,
                width = null,
                contentText = d["d[contentText]"],
                pathUrl = d["d[pathUrl]"],
                additionalServices = d["d[additionalServices]"],
                senderFromContract = false,
                sms = false,
                testoSms = d["d[testoSms]"],
                logo = d["d[logo]"],
                id = 0,
            };

            var j = JsonConvert.SerializeObject(l);

            int id = 0;
            var url = "Lol/NewOne?operationId=" + operationId + "&tsc=" + tsc + "&frc=" + frc + "&frm=false&userId=" + u.id + "&TipoLettera=" + TipoLettera;

            HttpResponseMessage get = await Globals.HttpClientSend("POST", url, u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();

            if (id == 0)
                return 0;

            return id;
        }

        public async Task<bool> Import(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var n = JsonConvert.DeserializeObject<UploadListResponse>(Request.Cookies["UploadResponse" + u.id].Value);
            GetRecipentLists ln = new GetRecipentLists();
            n.NamesLists.ToList().ForEach(a => a.listId = id);
            ln.recipient = n.NamesLists.Where(a => a.valid == true).ToList();

            foreach(var l in ln.recipient)
            {
                var get = await Globals.HttpClientSend("POST", "NamesLists/New", u.areaTestUser, l);
                if (!get.IsSuccessStatusCode)
                    return false;
                

            }

            return true;

            ViewBag.listId = id;


        }

        public async Task<ActionResult> NewPwd(int id, string id2)
        {
            var u = new Users();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Users/" + id, false);
            if (get.IsSuccessStatusCode)
                u = await get.Content.ReadAsAsync<Users>();
            if (u == null)
                return Redirect("/Home/Error500");
            if (u.guidUser.ToString() != id2)
                return Redirect("/Home/Error500");

            ViewBag.userId = u.id;
            return View();
        }

        public async Task<ActionResult> AutoLogin(string id)
        {
            if (Request.Cookies["login"] != null)
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["permits"] != null)
                Response.Cookies["permits"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["allPermits"] != null)
                Response.Cookies["allPermits"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["login"] == null)
            {
                var us = new Users();
                HttpResponseMessage get = await Globals.HttpClientSend("GET", "Users/" + id, false);
                if (get.IsSuccessStatusCode)
                    us = await get.Content.ReadAsAsync<Users>();

                GetUserAndPermissions u = new GetUserAndPermissions();
                var l = new Login()
                {
                    email = us.email,
                    pwd = us.pwd
                };
                
                get = await Globals.HttpClientSend("POST", "Users/LoginNew", false, l);
                if (get.IsSuccessStatusCode)
                    u = await get.Content.ReadAsAsync<GetUserAndPermissions>();

                if (u.user != null)
                {
                    var json = JsonConvert.SerializeObject(u.user);
                    var permits = JsonConvert.SerializeObject(u.permits);
                    Response.Cookies["login"].Value = json;
                    Response.Cookies["permits"].Value = permits;
                    Response.Cookies["allPermits"].Value = u.all.ToString();

                    if (!u.user.changePwd)
                        return RedirectToAction("ChangePwd");

                    return RedirectToAction("Dashboard");
                }
            }
            ViewBag.error = "Username o password inesistenti!";
            return Redirect("Index");
        }

        public ActionResult Error500()
        {
            return View();
        }

        public async Task<ActionResult> Dashboard()
        {

            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if(!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            List<Notifications> Notifications = new List<Notifications>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Notifications?userId=" + u.id + "&enabled=true", u.areaTestUser);
            if (get.IsSuccessStatusCode)
                Notifications = await get.Content.ReadAsAsync<List<Notifications>>();


            var d = new DashboardViewModel()
            {
                user = u,
                Notifications = Notifications.Where(a => a.notificationType == (int)notificationType.notification).ToList(),
                Communication = Notifications.Where(a => a.notificationType == (int)notificationType.communication).ToList()
            };

            return View(d);
        }

        public ActionResult Marketing()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            return View();
        }

        public ActionResult ChangePwd()
        {
            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            return View(u);
        }

        public ActionResult Account()
        {
            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            return View(u);
        }

        public ActionResult Pacchi()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            return View();
        }

        public async Task<ActionResult> Logout()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);
            await Globals.HttpClientSend("GET", "Users/LogOut?userId=" + u.id, false);

            if (Request.Cookies["login"] != null)
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["permits"] != null)
                Response.Cookies["permits"].Expires = DateTime.Now.AddDays(-1);

            if (Request.Cookies["allPermits"] != null)
                Response.Cookies["allPermits"].Expires = DateTime.Now.AddDays(-1);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Login(FormCollection dt)
        {
            if (Request.Cookies["login"] == null)
            {
                GetUserAndPermissions u = new GetUserAndPermissions();
                var l = new Login()
                {
                    email = dt["username"],
                    pwd = dt["pwd"]
                };
                HttpResponseMessage get = await Globals.HttpClientSend("POST", "Users/LoginNew", false, l);
                if (get.IsSuccessStatusCode)
                    u = await get.Content.ReadAsAsync<GetUserAndPermissions>();

                if (u.user != null)
                {
                    if (!u.user.abilitato)
                    {
                        ViewBag.error = "Utente disabilitato!<br>Contattare l'amministrazione.";
                        return View("Index");
                    }

                    var json = JsonConvert.SerializeObject(u.user);
                    var permits = JsonConvert.SerializeObject(u.permits);
                    Response.Cookies["login"].Value = json;
                    Response.Cookies["permits"].Value = permits;
                    Response.Cookies["allPermits"].Value = u.all.ToString();

                    if (!u.user.changePwd)
                        return RedirectToAction("ChangePwd");

                    return RedirectToAction("Dashboard");
                }
            }
            ViewBag.error = "Username o password inesistenti!";
            return View("Index");
        }

        public async Task<ActionResult> Recovery(FormCollection dt)
        { 
            var u = new Users();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Users/Recovery?email=" + dt["username"], false);
            if (get.IsSuccessStatusCode) { 
                u = await get.Content.ReadAsAsync<Users>();
                if(u == null)
                    ViewBag.recoveryMsg = "Nessuna email corrispondente nel nostro archivio.";
                else 
                {
                    var body = "Ciao, clicca sul link per recuperare la tua password.<br><a href='" + Globals.staticUrl + "/Home/NewPwd/" + u.id + "/" + u.guidUser.ToString() + "'> clicca qui</a>";
                    await Globals.sendEmail(u.email, "RECUPERO PASSWORD", body);
                    ViewBag.recoveryMsg = "Email inviata all'indirizzo specificato.";
                }
            }

            return View("PwdRecovery");
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditPersonalData(FormCollection dt)
        {
            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            u.name = dt["nome"];
            u.lastName = dt["cognome"];
            u.businessName = dt["ragsoc"];
            u.address = dt["indirizzo"];
            u.cap = dt["cap"];
            u.city = dt["citta"];
            u.province = dt["provincia"];
            u.mobile = dt["telefono"];
            u.name = dt["nome"];

            HttpResponseMessage get = await Globals.HttpClientSend("POST", "Users/Update/" + u.id, u.areaTestUser, u);

            var json = JsonConvert.SerializeObject(u);
            Response.Cookies["login"].Value = json;

            ViewBag.modificheSalvate = "Modifiche salvate con successo!";

            string description = "Modifica dati personali utente id " + u.id;

            int logType = (int)LogType.modPersonalData;
            await Globals.SetLogs(logType, u.id, description);


            return new HttpStatusCodeResult(200, "Richiesta inviata correttamente");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> EditPersonalPwd(FormCollection dt)
        {
            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            u.pwd = dt["newpassword"];
            u.changePwd = true;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Users/ChangePwd?pwd=" + u.pwd + "&userId=" + u.id, u.areaTestUser);

            var json = JsonConvert.SerializeObject(u);
            Response.Cookies["login"].Value = json;

            ViewBag.modificheSalvate = "Modifiche salvate con successo!";
            return new HttpStatusCodeResult(200, "Modifiche salvate correttamente");
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SavePwd(FormCollection dt)
        {
            var u = new Users();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Users/" + dt["id"], false);
            if (get.IsSuccessStatusCode)
                u = await get.Content.ReadAsAsync<Users>();

            u.pwd = dt["newpassword"];

            get = await Globals.HttpClientSend("POST", "Users/Update/" + u.id, u.areaTestUser, u);

            ViewBag.msg = "Modifiche salvate con successo!";

            string description = "Modifica password utente id " + u.id;

            int logType = (int)LogType.modPersonalData;
            await Globals.SetLogs(logType, u.id, description);

            return View("NewPwd");

        }
    }
}