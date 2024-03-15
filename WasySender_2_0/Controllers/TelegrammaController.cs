using AutoMapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WasySender_2_0.DataModel;
using WasySender_2_0.DataModel.SubmitRolResponse;
using WasySender_2_0.Models;

namespace WasySender_2_0.Controllers
{
    public class TelegrammaController : Controller
    {
        // GET: Telegramma
        public async Task<ActionResult> Index()
        {

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var senders = new List<Sender>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                senders = await get.Content.ReadAsAsync<List<Sender>>();


            return View(senders);
        }

        public ActionResult GotoStep2(FormCollection dataForm)
        {
            if (Convert.ToInt32(dataForm["type"]) == (int)ProductType.singolo)
                return Redirect("DestinatarioSingolo?Sender=" + dataForm["Sender"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"]);

            return Redirect("DestinatariList?Sender=" + dataForm["Sender"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"]);
        }

        public ActionResult SelezionaDestinatari()
        {
            return View();
        }

        public async Task<ActionResult> DestinatariList()
        {

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

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

        public async Task<string> GetList(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var l = new GetLists();
            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "Lists/" + id + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return "";

            l = await get.Content.ReadAsAsync<GetLists>();
            return new JavaScriptSerializer().Serialize(l);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> CreateDestinatari(FormCollection dataForm)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);
            var d = dataForm;
            var ids = d["ids[]"];
            var i = ids.Split(',');

            var l = new Lists();
            l.name = "Duplicazione Lista caricamento";
            l.description = "Caricamento file in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;
            l.temporary = true;

            int id = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();
            if (id == 0)
                return "";

            get = await Globals.HttpClientSend("GET", "NamesLists/ReplaceLists?ids=" + ids + "&listId=" + id, u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return "";

            return Convert.ToString(id);
        }

        public ActionResult DestinatariNew()
        {
            return View();
        }

        public async Task<ActionResult> NewMessage()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var l = new GetLists();
            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "Lists/" + Request.QueryString["ListId"] + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });

            l = await get.Content.ReadAsAsync<GetLists>();

            ViewBag.guiduser = u.guidUser.ToString();
            ViewBag.userId = u.id;
            ViewBag.areaTestUser = u.areaTestUser;

            return View(l);
        }


        public async Task<string> CalcolaPreventivo(FormCollection dataForm)
        {
            int ListId = Convert.ToInt32(dataForm["ListId"]);
            int Sender = Convert.ToInt32(dataForm["Sender"]);
            string msg = dataForm["msg"];
            int RicevutaRitorno = Convert.ToInt32(dataForm["RicevutaRitorno"]);

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = new HttpResponseMessage();

            //SENDER
            var s = new Sender();
            get = await Globals.HttpClientSend("GET", "SendersUsers/" + Sender, u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return "";

            s = await get.Content.ReadAsAsync<Sender>();


            //RECIPIENTS
            var l = new GetLists();
            get = await Globals.HttpClientSend("GET", "Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return "";

            l = await get.Content.ReadAsAsync<GetLists>();

            SenderRecipientsTelegram srt = new SenderRecipientsTelegram();
            List<NamesTelegram> recipients = new List<NamesTelegram>();
            //ATTACCHED FILE TO RECIPIENTS
            foreach (var r in l.recipients)
            {
                var  n = Mapper.Map<Recipient, NamesTelegram>(r);
                recipients.Add(n);
            }

            if (recipients.Count == 0)
                return "";

            var sr = new SenderRecipientsTelegram()
            {
                sender = s,
                recipients = recipients,
                testo = msg
            };

            get = await Globals.HttpClientSend("POST", "Telegram/Submit?guidUser=" + u.guidUser.ToString() + "&ricevutaRitorno=" + RicevutaRitorno + "&autoConfirm=false&userId=" + u.id, u.areaTestUser, sr);
            if (!get.IsSuccessStatusCode)
                return "";

            GetOperationResponse gor = await get.Content.ReadAsAsync<GetOperationResponse>();
            decimal importoNetto = 0;
            decimal importoIva = 0;
            decimal importoTotale = 0;
            foreach (var sres in gor.ListGetSubmitResponse)
            {
                importoNetto += sres.prices.price;
                importoIva += sres.prices.vatPrice;
                importoTotale += sres.prices.totalPrice;
            }
            var t = new TotalNamesPrice()
            {
                importoTotale = Math.Round(importoTotale, 2),
                importoIva = Math.Round(importoIva, 2),
                importoNetto = Math.Round(importoNetto, 2),
                numberOfNames = gor.ListGetSubmitResponse.Count(),
                operationId = gor.operationId
            };

            Session["TotalNamesPrice"] = t;
            
            return new JavaScriptSerializer().Serialize(t);
        }

        public async Task<ActionResult> Spedisci(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "Telegram/Confirm?guidUser=" + u.guidUser.ToString() + "&operationId=" + id, u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return Redirect("/Home/Error500");

            return Redirect("StepEnd?id=" + id);
        }


        public async Task<ActionResult> StepEnd(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/" + id + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                Response.Redirect("/Home/Error500");

            ViewBag.guiduser = u.guidUser.ToString();

            var go = await get.Content.ReadAsAsync<GetOperations>();

            int logType = (int)LogType.sendTol;
            string description = "Richiesta nuova TOL id " + go.operationId + " confermata.";
            await Globals.SetLogs(logType, u.id, description);

            ViewBag.areaTestUser = u.areaTestUser;

            return View(go);
        }
        public ActionResult DestinatarioSingolo()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.userId = u.id;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.areaTestUser = u.areaTestUser;
            return View();
        }

        public async Task<string> ValidazioneMittenteDestinatario(FormCollection c)
        {
            var RagioneSocialeMittente = c["ragsocSender"];
            var NomeMittente = c["nomeSender"];
            var CognomeMittente = c["cognomeSender"];
            var DugMittente = c["dugSender"];
            var IndirizzoMittente = c["indirizzoSender"];
            var NumeroCivicoMittente = c["numeroCivicoSender"];
            var CapMittente = c["capSender"];
            var CittaMittente = c["cittaSender"];
            var ProvinciaMittente = c["provinciaSender"];
            var StatoMittente = c["statoSender"];

            var RagioneSocialeDestinatario = c["ragsocNames"];
            var NomeDestinatario = c["nomeNames"];
            var CognomeDestinatario = c["cognomeNames"];
            var DugDestinatario = c["dugNames"];
            var IndirizzoDestinatario = c["indirizzoNames"];
            var NumeroCivicoDestinatario = c["numeroCivicoNames"];
            var CapDestinatario = c["capNames"];
            var CittaDestinatario = c["cittaNames"];
            var ProvinciaDestinatario = c["provinciaNames"];
            var StatoDestinatario = c["statoNames"];

            Sender s = new Sender()
            {
                businessName = RagioneSocialeMittente,
                name = NomeMittente,
                surname = CognomeMittente,
                dug = DugMittente,
                address = IndirizzoMittente,
                houseNumber = NumeroCivicoMittente,
                cap = CapMittente,
                city = CittaMittente,
                province = ProvinciaMittente,
                state = StatoMittente
            };

            var cM = new ControlloMittente();
            cM = CheckSender.verificaMittente(s);
            if (!cM.Valido)
                return new JavaScriptSerializer().Serialize(cM);


            var comuni = Globals.GetComuniList();

            NamesLists nl = new NamesLists()
            {
                businessName = RagioneSocialeDestinatario,
                name = NomeDestinatario,
                surname = CognomeDestinatario,
                dug = DugDestinatario,
                address = IndirizzoDestinatario,
                houseNumber = NumeroCivicoDestinatario,
                cap = CapDestinatario,
                city = CittaDestinatario,
                province = ProvinciaDestinatario,
                state = StatoDestinatario
            };

            var comune = comuni.Where(a => a.cap == CapDestinatario);

            var cD = new ControlloDestinatario();
            cD = CheckRecipient.verificaDestinatario(nl, comune);
            if (!cD.Valido)
                return new JavaScriptSerializer().Serialize(cD);


            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);


            HttpResponseMessage get = new HttpResponseMessage();

            //CREAZIONE MITTENTE
            int Sender = 0;
            s.userId = u.id;
            s.temporary = true;
            get = await Globals.HttpClientSend("POST", "SendersUsers/New", u.areaTestUser, s);
            if (get.IsSuccessStatusCode)
                Sender = await get.Content.ReadAsAsync<int>();


            //CREAZIONE DESTINATARIO
            //LISTA PROVVISORIA PER INSERIMENTO DESTINATARIO
            var l = new Lists();
            l.name = "Lista provvisoria singolo destinatario";
            l.description = "Lista in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;
            l.temporary = true;

            int listId = 0;
            get = await Globals.HttpClientSend("POST", "Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                listId = await get.Content.ReadAsAsync<int>();
            
            nl.listId = listId;

            get = await Globals.HttpClientSend("POST", "NamesLists/New", u.areaTestUser, nl);

            var rr = new RedirectSingleNames()
            {
                ListId = listId,
                Sender = Sender,
                RicevutaRitorno = c["RicevutaRitorno"],
                Valido = true
            };

            return new JavaScriptSerializer().Serialize(rr);
        }

        public ActionResult Preventivo()
        {
            return View();
        }

        public async Task<ActionResult> Riepilogo()
        {

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var t = new TotalNamesPrice();
            t = (TotalNamesPrice)Session["TotalNamesPrice"];

            int logType = (int)LogType.sendTol;
            string description = "Richiesta nuova TOL id " + t.operationId + " da confermare.";

            await Globals.SetLogs(logType, u.id, description);

            return View(t);
        }

    }
}