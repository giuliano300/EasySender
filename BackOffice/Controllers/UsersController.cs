using BackOffice.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using static BackOffice.Models.EnumClass;

namespace BackOffice.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Logs(int id)
        {
            ViewBag.id = id;
            return View();
        }

        public async Task<ActionResult> New()
        {
            ViewBag.HeaderPage = "Aggiungi";
            Users s = new Users();
            return View(s);
        }

        public async Task<ActionResult> Edit(int id)
        {
            ViewBag.HeaderPage = "Modifica";
            Users c = new Users();

            string r = "";

            HttpResponseMessage get = await Globals.HttpClientSend("GET", Globals.apiUri + "api/Users/" + id);
            if (get.IsSuccessStatusCode)
               r = await get.Content.ReadAsStringAsync();

            c = JsonConvert.DeserializeObject<Users>(r);

            if (c != null) { 
                ViewBag.dataScadenza = c.expireDate == null ? DateTime.Now.ToString("dd/MM/yyyy") : Convert.ToDateTime(c.expireDate).ToString("dd/MM/yyyy");

                c.insertDate = c.insertDate == null ? DateTime.Now : Convert.ToDateTime(c.insertDate);
            }

            return View("New", c);
        }

       public async Task<ActionResult> NewChild(int id)
        {
            Users c = new Users();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", Globals.apiUri + "api/Users/" + id);
            if (get.IsSuccessStatusCode)
                c = await get.Content.ReadAsAsync<Users>();

            ViewBag.parentId = c.id;
            ViewBag.HeaderPage = "Aggiungi";
            Users s = new Users();
            return View(s);
        }

        public async Task<ActionResult> EditChild(int id)
        {
            ViewBag.HeaderPage = "Modifica";
            Users c = new Users();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", Globals.apiUri + "api/Users/" + id);
            if (get.IsSuccessStatusCode)
                c = await get.Content.ReadAsAsync<Users>();

            ViewBag.parentId = c.parentId;

            return View("NewChild", c);
        }

        // GET: Users
        public ActionResult Children(int id)
        {
            ViewBag.parentId = id;
            return View();
        }



        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SaveParent(FormCollection dataForm)
        {
            Users c = new Users();
            if (ModelState.IsValid)
            {

                var data = dataForm;

                bool hide = false;

                var h = data["hidePrice"];
                if (h != null)
                    hide = true;

                var piva = data["piva"];
                var usernamePoste = data["usernamePoste"];
                var pwdPoste = data["pwdPoste"];
                var name = data["name"];
                var lastName = data["lastName"];
                var email = data["email"];
                var businessName = data["businessName"];
                var guidUser = data["guidUser"];
                var pwd = data["pwd"];
                var address = data["address"];
                var cap = data["cap"];
                var city = data["city"];
                var province = data["province"];
                var mobile = data["mobile"];
                var parentId = Convert.ToInt32(data["parentId"]);
                var areaTestUser = data["areaTestUser"];
                var mol = data["mol"];
                var col = data["col"];
                var ged = data["ged"];
                var sms = data["sms"];
                var pacchi = data["pacchi"];
                var abilitato = data["abilitato"];
                var master = data["master"];
                var collegatoMasterId = data["collegatoMasterId"];
                var conciliazioneBollettini = data["conciliazioneBollettini"];
                var rendicontazioneFatture = data["rendicontazioneFatture"];
                var downloadFile = data["downloadFile"];
                var changePwd = data["changePwd"];
                var CodiceContrattoMOL = data["CodiceContrattoMOL"];
                var CodiceContrattoCOL = data["CodiceContrattoCOL"];
                var usernameGED = data["usernameGED"];
                var passwordGED = data["passwordGED"];
                var usernamePacchi = data["usernamePacchi"];
                var passwordPacchi = data["passwordPacchi"];
                var centerCostPacchi = data["centerCostPacchi"];
                var userPriority = Convert.ToInt32(data["userPriority"]);
                var porpertyId = Convert.ToInt32(data["porpertyId"]);
                var id = data["id"];
                var senderPkgId = data["senderPkgId"];
                var fiscalCode = data["fiscalCode"];
                var emailPec = data["emailPec"];
                var insertDate = data["insertDate"];
                var expireDate = data["expireDate"];
                var sso = data["sso"];

                var CodiceContrattoAGOLM = data["CodiceContrattoAGOLM"];
                var CodiceContrattoAGOLB = data["CodiceContrattoAGOLB"];


                if (sso != null)
                {
                    c.sso = true;
                    c.changePwd = true;
                    c.userType = 3;
                }
                else
                {
                    c.userType = 1;
                }

                bool art = true;
                if (areaTestUser == null)
                    art = false;

                bool umol = true;
                if (mol == null)
                    umol = false;

                bool ucol = true;
                if (col == null)
                    ucol = false;

                bool uged = true;
                if (ged == null)
                    uged = false;

                bool umaster = true;
                if (master == null)
                    umaster = false;

                bool uabilitato = true;
                if (abilitato == null)
                    uabilitato = false;

                bool udownloadFile = true;
                if (downloadFile == null)
                    udownloadFile = false;

                bool uconciliazioneBollettini = true;
                if (conciliazioneBollettini == null)
                    uconciliazioneBollettini = false;

                bool urendicontazioneFatture = true;
                if (rendicontazioneFatture == null)
                    urendicontazioneFatture = false;

                bool upacchi = true;
                if (pacchi == null)
                    upacchi = false;

                bool usms = true;
                if (sms == null)
                    usms = false;

                c.name = name;
                c.usernamePoste = usernamePoste;
                c.pwdPoste = pwdPoste;
                c.name = name;
                c.lastName = lastName;
                
                c.email = email;
                c.businessName = businessName;
                c.pwd = pwd;
                c.address = address;
                c.cap = cap;
                c.city = city;
                c.province = province;
                c.mobile = mobile;
                c.parentId = parentId;
                c.usernamePosteAreaTest = usernamePoste;
                c.pwdPosteAreaTest = pwdPoste;
                c.areaTestUser = art;
                c.mol = umol;
                c.CodiceContrattoMOL = CodiceContrattoMOL;
                c.col = ucol;
                c.CodiceContrattoCOL = CodiceContrattoCOL;
                c.userPriority = (UserPriority)userPriority;
                c.downloadFile = udownloadFile;
                c.usernameGED = usernameGED;
                c.passwordGED = passwordGED;
                c.GED = uged;
                c.piva = piva;
                c.fiscalCode = fiscalCode;
                c.porpertyId = (Property)porpertyId;
                c.master = umaster;
                c.collegatoMasterId = collegatoMasterId;
                c.conciliazioneBollettini = uconciliazioneBollettini;
                c.rendicontazioneFatture = urendicontazioneFatture;
                c.abilitato = uabilitato;
                c.sms = usms;
                c.emailPec = emailPec;
                c.hidePrice = hide;

                c.Pacchi = upacchi;
                c.usernamePacchi = usernamePacchi;
                c.passwordPacchi = passwordPacchi;
                c.centerCostPacchi = centerCostPacchi;

                c.CodiceContrattoAGOLM = CodiceContrattoAGOLM;
                c.CodiceContrattoAGOLB = CodiceContrattoAGOLB;


                //Inserimento date 

                c.insertDate = insertDate == "" ? DateTime.Now : Convert.ToDateTime(insertDate);
                c.expireDate = expireDate == "" ? DateTime.Now : Convert.ToDateTime(expireDate);


                c.senderPkgId = 0;
                if (senderPkgId != "")
                    c.senderPkgId = (SenderPgkId)Convert.ToInt32(senderPkgId);



                if (id == "0")
                {
                    c.guidUser = Guid.NewGuid();
                    await Globals.HttpClientSend("POST", Globals.apiUri + "api/Users/New", c);
                }
                else
                {
                    c.id = Convert.ToInt32(id);
                    c.guidUser = Guid.Parse(guidUser);
                    //await Globals.HttpClientSend("POST", Globals.apiUri + "api/Users/Update/" + id, c);
                    await Globals.HttpClientSend("POST", Globals.apiUri + "api/Users/Update/" + id, c);
                }

            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SaveChild(FormCollection dataForm)
        {
            Users c = new Users();
            if (ModelState.IsValid)
            {
                var data = dataForm;

                Users p = new Users();
                HttpResponseMessage get = await Globals.HttpClientSend("GET", Globals.apiUri + "api/Users/" + data["parentId"]);
                if (get.IsSuccessStatusCode)
                    p = await get.Content.ReadAsAsync<Users>();

                bool hide = false;

                var h = data["hidePrice"];
                if (h != null)
                    hide = true;

                var name = data["name"];
                var lastName = data["lastName"];
                var userType = Convert.ToInt32(data["userType"]);
                var email = data["email"];
                var pwd = data["pwd"];
                var id = data["id"];
                var sso = data["sso"];


                if (sso != null)
                {
                    c.sso = true;
                    c.changePwd = true;
                    c.userType = 3;
                }
                else
                {
                    c.userType = 1;
                }


                c.email = email;
                c.pwd = pwd;
                c.name = name;
                c.lastName = lastName;
                c.userType = userType;

                c.usernamePoste = p.usernamePoste;
                c.pwdPoste = p.pwdPoste;
                c.name = name;
                c.userType = userType;

                c.businessName = p.businessName;
                c.address = p.address;
                c.cap = p.cap;
                c.city = p.city;
                c.province = p.province;
                c.mobile = p.mobile;
                c.parentId = p.id;
                c.usernamePosteAreaTest = p.usernamePoste;
                c.pwdPosteAreaTest = p.pwdPoste;
                c.areaTestUser = p.areaTestUser;
                c.mol = p.mol;
                c.CodiceContrattoMOL = p.CodiceContrattoMOL;
                c.col = p.col;
                c.CodiceContrattoCOL = p.CodiceContrattoCOL;
                c.userPriority = p.userPriority;
                c.guidUser = p.guidUser;
                c.downloadFile = p.downloadFile;
                c.abilitato = p.abilitato;
                c.sms = p.sms;
                c.hidePrice = hide;

                c.CodiceContrattoAGOLM = p.CodiceContrattoAGOLM;
                c.CodiceContrattoAGOLB = p.CodiceContrattoAGOLB;

                if (id == "0")
                {
                    await Globals.HttpClientSend("POST", Globals.apiUri + "api/Users/New", c);
                }
                else
                {
                    c.id = Convert.ToInt32(id);
                    await Globals.HttpClientSend("POST", Globals.apiUri + "api/Users/Update/" + id, c);
                }

            }
            return RedirectToAction("Children/" + dataForm["parentId"]);

        }


    }
}