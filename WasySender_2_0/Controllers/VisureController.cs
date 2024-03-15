using AutoMapper;
using Newtonsoft.Json;
using Rotativa;
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

namespace WasySender_2_0.Controllers
{
    public class VisureController : Controller
    {
        // GET: Visure
        public async Task<ActionResult> Index(int id)
        {

            ViewBag.ProductTypeName = "";

            switch (id)
            {
                case 0:
                    ViewBag.ProductTypeName = "multipla";
                    break;
                case 2:
                    ViewBag.ProductTypeName = "singola";
                    break;
            }

            ViewBag.ProductTypeId = id;

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var senders = new List<Sender>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                senders = await get.Content.ReadAsAsync<List<Sender>>();

            return View(senders);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> SearchTipoDocumento(FormCollection fc)
        {
            var tipoDocumento = fc["tipoDocumento"];
            var c = new List<CompleteDocsVisure>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/VisureDocumentType?documentType=" + tipoDocumento, false);
            if (get.IsSuccessStatusCode)
                c = await get.Content.ReadAsAsync<List<CompleteDocsVisure>>();

            return new JavaScriptSerializer().Serialize(c);
        }

        public async Task<ActionResult> GotoStep2(FormCollection dataForm)
        {

            if (Convert.ToInt32(dataForm["id"]) == (int)ProductType.singolo)
                return Redirect("IntestatarioSingolo?Sender=" + dataForm["Sender"] + "&TipoDocumento=" + dataForm["tipoDocumento"] + "&CodiceDocumento=" + dataForm["codiceDocumento"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"]);


            int SenderAR = 0;

            if (dataForm["RicevutaRitorno"] == "1")
            {
                //ESISTE LA RICEVUTA DI RITORNO
                //INSERISCO IL DESTINATARIO AR
                var NomeMittenteAR = dataForm["nameSenderAR"];
                var CognomeMittenteAR = dataForm["surnameSenderAR"];
                var IndirizzoMittenteAR = dataForm["indirizzoSenderAR"];
                var CompletamentoIndirizzoMittenteAR = dataForm["completamentoIndirizzoSenderAR"];
                var CapMittenteAR = dataForm["capSenderAR"];
                var CittaMittenteAR = dataForm["cittaSenderAR"];
                var ProvinciaMittenteAR = dataForm["provinciaSenderAR"];
                var StatoMittenteAR = dataForm["statoSenderAR"];
                var TelefonoAR = dataForm["telefonoSenderAR"];
                var EmailAR = dataForm["emailSenderAR"];

                var sAR = new Sender()
                {
                    name = NomeMittenteAR,
                    surname = CognomeMittenteAR,
                    complementAddress = CompletamentoIndirizzoMittenteAR,
                    address = IndirizzoMittenteAR,
                    cap = CapMittenteAR,
                    city = CittaMittenteAR,
                    province = ProvinciaMittenteAR,
                    state = StatoMittenteAR,
                    telefono = TelefonoAR,
                    email = EmailAR
                };

                var cMAR = new ControlloMittente();
                cMAR = CheckSender.verificaMittente(sAR);
                if (!cMAR.Valido)
                    return Redirect("/Home/Error500");


                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                HttpResponseMessage get = new HttpResponseMessage();

                //CREAZIONE MITTENTE AR
                sAR.temporary = true;
                sAR.userId = u.id;
                get = await Globals.HttpClientSend("POST", "/api/SendersUsers/New", u.areaTestUser, sAR);
                if (get.IsSuccessStatusCode)
                    SenderAR = await get.Content.ReadAsAsync<int>();

            }

            return Redirect("IntestatariNew?Sender=" + dataForm["Sender"] + "&SenderAR=" + SenderAR + "&TipoDocumento=" + dataForm["tipoDocumento"] + "&codiceDocumento=" + dataForm["CodiceDocumento"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"]);
        }

        public ActionResult IntestatarioSingolo()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.userId = u.id;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.areaTestUser = u.areaTestUser;
            return View();
        }
        public async Task<string> ValidazioneRichiedenteIntestatario(FormCollection c)
        {
            var NomeMittente = c["nameSender"];
            var CognomeMittente = c["surnameSender"];
            var IndirizzoMittente = c["indirizzoSender"];
            var CapMittente = c["capSender"];
            var CittaMittente = c["cittaSender"];
            var ProvinciaMittente = c["provinciaSender"];
            var StatoMittente = c["statoSender"];
            var TelefonoMittente = c["telefonoSender"];
            var EmailMittente = c["emailSender"];

            var NominativoDestinatario = c["nominativoNames"];
            var IndirizzoDestinatario = c["indirizzoNames"];
            var CapDestinatario = c["capNames"];
            var CittaDestinatario = c["cittaNames"];
            var ProvinciaDestinatario = c["provinciaNames"];
            var StatoDestinatario = c["statoNames"];
            var NREADestitatario = c["NREANames"];
            var CodiceFiscaleDestitatario = c["fiscalCodeNames"];
            var NomeFile = "";
            var selectedNamesListId = c["selectedNamesListId"];
            var namesId = c["namesId"];
            var TipoDocumento = Convert.ToInt32(c["TipoDocumento"]);
            var CodiceDocumento = Convert.ToInt32(c["CodiceDocumento"]);


            Sender s = new Sender()
            {
                name = NomeMittente,
                surname = CognomeMittente,
                address = IndirizzoMittente,
                cap = CapMittente,
                city = CittaMittente,
                province = ProvinciaMittente,
                state = StatoMittente,
                telefono = TelefonoMittente,
                email = EmailMittente
            };

            var cM = new ControlloMittente();
            cM = CheckSender.verificaMittente(s);
            if (!cM.Valido)
                return new JavaScriptSerializer().Serialize(cM);

            //DESTINATARIO RICEVUTA DI RITORNO
            //DIVERSO DAL MITTENTE
            Sender sAR = null;
            var AR = c["addMittenteAR"];
            if (c["RicevutaRitorno"] != "0")
                if (AR == null)
                {
                    var NomeMittenteAR = c["nameSenderAR"];
                    var CognomeMittenteAR = c["surnameSenderAR"];
                    var CompletamentoNominativoMittenteAR = c["complementoNominativoSenderAR"];
                    var IndirizzoMittenteAR = c["indirizzoSenderAR"];
                    var CompletamentoIndirizzoMittenteAR = c["completamentoIndirizzoSenderAR"];
                    var CapMittenteAR = c["capSenderAR"];
                    var CittaMittenteAR = c["cittaSenderAR"];
                    var ProvinciaMittenteAR = c["provinciaSenderAR"];
                    var StatoMittenteAR = c["statoSenderAR"];
                    var TelefonoMittenteAR = c["telefonoSenderAR"];
                    var EmailMittenteAR = c["emailSenderAR"];

                    sAR = new Sender()
                    {
                        name = NomeMittenteAR,
                        surname = CognomeMittenteAR,
                        address = IndirizzoMittenteAR,
                        cap = CapMittenteAR,
                        city = CittaMittenteAR,
                        province = ProvinciaMittenteAR,
                        state = StatoMittenteAR,
                        telefono = TelefonoMittenteAR,
                        email = EmailMittenteAR
                    };

                    var cMAR = new ControlloMittente();
                    cMAR = CheckSender.verificaMittente(sAR);
                    if (!cMAR.Valido)
                        return new JavaScriptSerializer().Serialize(cMAR);
                };

            var comuni = Globals.GetComuniList();

            NamesLists nl = new NamesLists()
            {
                businessName = NominativoDestinatario,
                address = IndirizzoDestinatario,
                cap = CapDestinatario,
                city = CittaDestinatario,
                province = ProvinciaDestinatario,
                state = StatoDestinatario,
                fileName = NomeFile,
                NREA = NREADestitatario,
                fiscalCode = CodiceFiscaleDestitatario,
                tipoDocumento = TipoDocumento,
                codiceDocumento = CodiceDocumento
            };

            var comune = comuni.Where(a => a.cap == CapDestinatario);

            var cD = new ControlloDestinatario();
            cD = await CheckRecipient.verificaDestinatarioVisure(nl);
            if (!cD.Valido)
                return new JavaScriptSerializer().Serialize(cD);

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);


            HttpResponseMessage get = new HttpResponseMessage();

            //CREAZIONE MITTENTE
            int Sender = 0;
            int SenderAR = 0;
            s.userId = u.id;

            var t = true;
            if (c["addRubrica"] == "on")
                t = false;

            s.temporary = t;
            get = await Globals.HttpClientSend("POST", "/api/SendersUsers/New", u.areaTestUser, s);
            if (get.IsSuccessStatusCode)
                Sender = await get.Content.ReadAsAsync<int>();


            if (c["RicevutaRitorno"] != "0")
                if (AR == null)
                {
                    sAR.temporary = true;
                    sAR.userId = u.id;
                    get = await Globals.HttpClientSend("POST", "/api/SendersUsers/New", u.areaTestUser, sAR);
                    if (get.IsSuccessStatusCode)
                        SenderAR = await get.Content.ReadAsAsync<int>();
                }

            int listId = 0;
            if (Convert.ToInt32(selectedNamesListId) > 0)
                nl.noUse = true;

            //CREAZIONE DESTINATARIO
            //LISTA PROVVISORIA PER INSERIMENTO DESTINATARIO
            var l = new Lists();
            l.name = "Lista provvisoria singolo destinatario";
            l.description = "Lista in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;
            l.temporary = true;

            get = await Globals.HttpClientSend("POST", "/api/Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                listId = await get.Content.ReadAsAsync<int>();


            //INSERIMENTO INTESTATARIO
            GetRecipentLists ln = new GetRecipentLists();
            nl.listId = listId;

            var r = new List<NamesLists>();
            r.Add(nl);
            ln.recipient = r;

            get = await Globals.HttpClientSend("POST", "/api/NamesLists/NewMultipleWithBulletin", u.areaTestUser, ln);

            var rr = new RedirectSingleNamesVisure()
            {
                ListId = listId,
                Sender = Sender,
                SenderAR = SenderAR,
                TipoDocumento = c["TipoDocumento"],
                CodiceDocumento = c["CodiceDocumento"],
                RicevutaRitorno = c["RicevutaRitorno"],
                Valido = true
            };

            return new JavaScriptSerializer().Serialize(rr);
        }
        public async Task<ActionResult> GeneraPreventivoSingoloIntestatario()
        {
            try
            {
                int ListId = Convert.ToInt32(Request.QueryString["ListId"]);
                int Sender = Convert.ToInt32(Request.QueryString["Sender"]);
                int SenderAR = Convert.ToInt32(Request.QueryString["SenderAR"]);
                int CodiceDocumento = Convert.ToInt32(Request.QueryString["CodiceDocumento"]);
                int TipoDocumento = Convert.ToInt32(Request.QueryString["TipoDocumento"]);
                int RicevutaRitorno = Convert.ToInt32(Request.QueryString["RicevutaRitorno"]);

                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                HttpResponseMessage get = new HttpResponseMessage();

                //SENDER
                var s = new Sender();
                get = await Globals.HttpClientSend("GET", "api/SendersUsers/" + Sender, u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");

                s = await get.Content.ReadAsAsync<Sender>();

                //SENDERAR
                object sAR = null;
                if (SenderAR > 0)
                {
                    get = await Globals.HttpClientSend("GET", "api/SendersUsers/" + SenderAR, u.areaTestUser);
                    if (!get.IsSuccessStatusCode)
                        return Redirect("/Home/Error500");

                    sAR = await get.Content.ReadAsAsync<Sender>();

                }

                //RECIPIENTS
                var l = new GetLists();
                get = await Globals.HttpClientSend("GET", "api/Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");

                l = await get.Content.ReadAsAsync<GetLists>();

                List<GetRecipent> recipents = new List<GetRecipent>();
                //ATTACCHED FILE TO RECIPIENTS
                foreach (var r in l.recipients)
                {
                    GetRecipent gr = new GetRecipent();
                    r.fileName = null;
                    Names n = new Names();
                    n = Mapper.Map<Recipient, Names>(r);
                    gr.recipient = n;
                    recipents.Add(gr);
                }

                if (recipents.Count == 0)
                    return Redirect("/Home/Error500");

                var sr = new SenderRecipients()
                {
                    sender = s,
                    senderAR = (Sender)sAR,
                    recipients = recipents
                };

                var url = "/api/Vol/CheckAllFiles";


                get = await Globals.HttpClientSend("POST", url + "?guidUser=" + u.guidUser.ToString() + "&CodiceDocumento=" +
                  CodiceDocumento + "&TipoDocumento=" + TipoDocumento + "&rrc=" + Convert.ToBoolean(RicevutaRitorno) + "&userId=" + u.id, u.areaTestUser, sr);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");

                GetNumberOfCheckedNames gor = await get.Content.ReadAsAsync<GetNumberOfCheckedNames>();
                decimal importoNetto = 0;
                decimal importoIva = 0;
                decimal importoTotale = 0;
                foreach (var sres in gor.checkedNames)
                {
                    importoNetto += sres.price.price;
                    importoIva += sres.price.vatPrice;
                    importoTotale += sres.price.totalPrice;
                }
                var t = new TotalNamesPrice()
                {
                    importoTotale = Math.Round(importoTotale, 2),
                    importoIva = Math.Round(importoIva, 2),
                    importoNetto = Math.Round(importoNetto, 2),
                    numberOfNames = gor.numberOfValidNames,
                    operationId = gor.operationId
                };

                Session["TipoDocumento"] = TipoDocumento;
                Session["TotalNamesPriceVol"] = t;

                return View("Preventivo");
            }
            catch (Exception e)
            {
                return Redirect("/Home/Error500");
            }

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
            t = (TotalNamesPrice)Session["TotalNamesPriceVol"];

            ViewBag.tipoDocumento = "CERTIFICATO";
            if (Session["TipoDocumento"].ToString() == "1")
                ViewBag.tipoDocumento = "VISURA";

            int logType = (int)LogType.sendVis;
            string description = "Richiesta nuova VOL id " + t.operationId + " da confermare.";

            await Globals.SetLogs(logType, u.id, description);

            return View(t);
        }
        public async Task<ActionResult> StepEnd(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "api/Operations/SetComplete?id=" + id + "&guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                Response.Redirect("/Home/Error500");

            ViewBag.guiduser = u.guidUser.ToString();

            var go = await get.Content.ReadAsAsync<GetOperations>();

            int logType = (int)LogType.sendVis;
            string description = "Richiesta nuova VOL id " + go.operationId + " confermata.";

            await Globals.SetLogs(logType, u.id, description);


            return View(go);
        }


        public ActionResult IntestatariNew()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.userId = u.id;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.areaTestUser = u.areaTestUser;
            return View();
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> UploadList()
        {
            try
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

                        UploadListResponse res = null;
                        //CONTROLLO CSV
                        res = await Globals.ReadCsvVisureShowResults(dbDirectory + name, u, Session.SessionID);

                        Session["UploadResponse"] = res;
                        return new JavaScriptSerializer().Serialize(res);

                    }
                }

                return new JavaScriptSerializer().Serialize(r);
            }
            catch (Exception e)
            {
                return e.InnerException.ToString();
            }
        }


        [HttpPost]
        [ValidateInput(false)]
        public string GetNames(FormCollection fc)
        {
            int index = Convert.ToInt32(fc["index"]);

            UploadListResponse n = (UploadListResponse)Session["UploadResponse"];

            var nb = new List<object>();
            var namelist = n.NamesLists.Select((c, i) => new { NamesLists = c, Index = i }).Where(x => x.Index == index);
            var ne = namelist.FirstOrDefault();
            NamesLists name = ne.NamesLists;
            nb.Add(name);

            Bulletins b = new Bulletins();
            if (n.Bulletins != null)
            {
                var bulletins = n.Bulletins.Select((c, i) => new { Bulletins = c, Index = i }).Where(x => x.Index == index);
                var be = bulletins.FirstOrDefault();
                b = be.Bulletins;
                nb.Add(b);
            }

            return new JavaScriptSerializer().Serialize(nb);
        }

        public async Task<string> ValidateName(FormCollection fc)
        {
            var comuni = Globals.GetComuniList();

            NamesLists s = new NamesLists()
            {
                businessName = fc["nominativo"],
                province = fc["provincia"],
                surname = fc["cognome"],
                fiscalCode = fc["fiscalCode"],
                NREA = fc["NREA"]
            };

            var c = new ControlloDestinatario();
            c = await CheckRecipient.verificaDestinatarioVisure(s);

            return new JavaScriptSerializer().Serialize(c);
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
                businessName = fc["nominativo"],
                province = fc["provincia"],
                surname = fc["cognome"],
                fiscalCode = fc["fiscalCode"],
                NREA = fc["NREA"],
                listId = 0,
                id = 0,
                valid = true
            };

            List<NamesLists> ls = new List<NamesLists>();

            var l = n.NamesLists.ToList();

            for (var i = 0; i < l.Count(); i++)
            {
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

        public async Task<ActionResult> GeneraPreventivo(FormCollection dataForm)
        {
            try
            {
                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                int ListId = 0;
                int Sender = Convert.ToInt32(dataForm["Sender"]);
                int SenderAR = Convert.ToInt32(dataForm["SenderAR"]);
                int CodiceDocumento = Convert.ToInt32(dataForm["CodiceDocumento"]);
                int TipoDocumento = Convert.ToInt32(dataForm["TipoDocumento"]);
                int RicevutaRitorno = Convert.ToInt32(dataForm["RicevutaRitorno"]);

                var l = new Lists();
                l.name = "Lista caricamento";
                l.description = "Caricamento file in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
                l.userId = u.id;
                l.date = DateTime.Now;

                HttpResponseMessage get = await Globals.HttpClientSend("POST", "/api/Lists/New", u.areaTestUser, l);
                if (get.IsSuccessStatusCode)
                    ListId = await get.Content.ReadAsAsync<int>();

                if (ListId == 0)
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });

                UploadListResponse n = (UploadListResponse)Session["UploadResponse"];
                GetRecipentLists ln = new GetRecipentLists();
                n.NamesLists.ToList().ForEach(a => a.listId = ListId);
                ln.recipient = n.NamesLists.Where(a => a.valid == true).ToList();

                get = await Globals.HttpClientSend("POST", "/api/NamesLists/NewMultipleWithBulletin", u.areaTestUser, ln);
                if (!get.IsSuccessStatusCode)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });
                }

                //SENDER
                var s = new Sender();
                get = await Globals.HttpClientSend("GET", "api/SendersUsers/" + Sender, u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });
                }
                s = await get.Content.ReadAsAsync<Sender>();

                //SENDER AR
                object sAR = null;
                if (SenderAR != 0)
                {
                    get = await Globals.HttpClientSend("GET", "api/SendersUsers/" + Convert.ToInt32(SenderAR), u.areaTestUser);
                    if (!get.IsSuccessStatusCode)
                    {
                        return RedirectToRoute(new
                        {
                            controller = "Home",
                            action = "Error500"
                        });
                    }
                    sAR = await get.Content.ReadAsAsync<Sender>();
                }

                //RECIPIENTS
                var gl = new GetLists();
                get = await Globals.HttpClientSend("GET", "api/Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });
                };

                gl = await get.Content.ReadAsAsync<GetLists>();

                List<GetRecipent> recipents = new List<GetRecipent>();
                foreach (var r in gl.recipients)
                {
                    GetRecipent gr = new GetRecipent();
                    r.fileName =null;
                    Names na = new Names();
                    na = Mapper.Map<Recipient, Names>(r);
                    gr.recipient = na;
                    recipents.Add(gr);
                };

                if (recipents.Count == 0)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });
                };

                var sr = new SenderRecipients()
                {
                    sender = s,
                    senderAR = (Sender)sAR,
                    recipients = recipents
                };

                var url = "/api/Vol/CheckAllFiles";

                get = await Globals.HttpClientSend("POST", url + "?guidUser=" + u.guidUser.ToString() + "&CodiceDocumento=" +
                  CodiceDocumento + "&TipoDocumento=" + TipoDocumento + "&rrc=" + Convert.ToBoolean(RicevutaRitorno) + "&userId=" + u.id, u.areaTestUser, sr);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");


                GetNumberOfCheckedNames gor = await get.Content.ReadAsAsync<GetNumberOfCheckedNames>();
                decimal importoNetto = 0;
                decimal importoIva = 0;
                decimal importoTotale = 0;
                foreach (var sres in gor.checkedNames)
                {
                    importoNetto += sres.price.price;
                    importoIva += sres.price.vatPrice;
                    importoTotale += sres.price.totalPrice;
                }
                var t = new TotalNamesPrice()
                {
                    importoTotale = Math.Round(importoTotale, 2),
                    importoIva = Math.Round(importoIva, 2),
                    importoNetto = Math.Round(importoNetto, 2),
                    numberOfNames = gor.numberOfValidNames,
                    operationId = gor.operationId
                };

                Session["TipoDocumento"] = TipoDocumento;
                Session["TotalNamesPriceVol"] = t;

                return View("Preventivo");
            }
            catch (Exception e)
            {
                return Redirect("/Home/Error500");
            }
        }

        public async Task<ActionResult> Richieste()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            ViewBag.guiduser = u.guidUser;

            var GetOperations = new List<GetOperations>();

            var filter = "&operationType=6";
            if (u.parentId > 0)
                filter = "&userId=" + u.id;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations?guidUser=" + u.guidUser + filter, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<List<GetOperations>>();

              ViewBag.productType = "VISURE/CERTIFICATI";

            GetOperations = GetOperations.ToList();

            await Globals.SetLogs((int)LogType.visVis, u.id, "Visualizzazione richiesta Visure / Certificati");


            return View(GetOperations);
        }

        public async Task<ActionResult> DettaglioRichiesta(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.guiduser = u.guidUser;
            ViewBag.userName = (u.businessName != "" ? u.businessName : u.name + " " + u.lastName);

            var GetOperationsNew = new GetOperationsNew();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations/Items/" + id + "?guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperationsNew = await get.Content.ReadAsAsync<GetOperationsNew>();

            return View(GetOperationsNew);
        }

        public ActionResult ExportPDF(int id)
        {
            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            return new ActionAsPdf("Distinta", new { id = id, guidUser = u.guidUser.ToString(), areaTest = u.areaTestUser })
            {
                FileName = Server.MapPath("~/Download/distinta.pdf"),
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4
            };
        }

        public async Task<string> GetPdf(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var GetOperations = new GetOperations();
            var fileName = "";

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Names/GetFile?nameId=" + id + "&guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                fileName = await get.Content.ReadAsAsync<string>();

            if (fileName == "")
                return fileName;

            var url = Globals.downladFile;

            return url + fileName.Split('/')[3];
        }

        public async Task<ActionResult> ArchivioRichieste()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            ViewBag.guiduser = u.guidUser;
            ViewBag.userId = u.id;

            int logType = (int)LogType.requestArchive;
            string description = "Visualizzazione Archivio richieste visure";

            await Globals.SetLogs(logType, u.id, description);

            return View();
        }
        public async Task<FileContentResult> DownloadCSV(FormCollection df)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var stringConcat = "&userId=" + u.id;
            if (df["username"] != null)
                stringConcat += "&username=" + df["username"];
            if (df["code"] != null)
                stringConcat += "&code=" + df["code"];
            if (df["esito"] != null)
                stringConcat += "&esito=" + df["esito"];
            if (df["dataDa"] != null)
                stringConcat += "&dataDa=" + df["dataDa"];
            if (df["dataA"] != null)
                stringConcat += "&dataA=" + df["dataA"];
            if (df["pp"] != null)
                stringConcat += "&pp=" + df["pp"];

            var lgo = new List<GetOperations>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations/GetAllOperations?prodotto=6&guidUser=" + u.guidUser + stringConcat, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                lgo = await get.Content.ReadAsAsync<List<GetOperations>>();


            string csv = "TIPO;MITTENTE;INTESTATARIO;ESITO;DATA ACCETTAZIONE;PREZZO;CODICE;STATO\n";
            foreach (var go in lgo)
            {
                foreach (var r in go.recipients)
                {
                    csv += (r.tipoDocumento == 0 ? "CERTIFICATO" : "VISURE") + ";" + go.sender.businessName + " " + go.sender.name + " " + go.sender.surname + "; " + r.businessName + " " + r.name + " " + r.surname + ";" + (r.codice != "" ? "OK" : "ERRORE") + ";" + r.presaInCaricoDate + ";" + r.totalPrice + ";" + r.codice + "; " + (r.stato != null ? r.stato : "Stato non disponibile") + "\n";
                }
            }

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report-" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv");
        }

    }
}