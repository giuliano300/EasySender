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
using System.Web.Script.Serialization;
using WasySender_2_0.DataModel;
using AutoMapper;
using AutoMapper.Internal;

namespace WasySender_2_0.Controllers
{
    public class RaccomandataController : Controller
    {
        // GET: Raccomandata
        public async Task<ActionResult> Index(int id)
        {

            ViewBag.ProductTypeName = "";

            switch (id)
            {
                case 0:
                    ViewBag.ProductTypeName = "semplice";
                    break;
                case 1:
                    ViewBag.ProductTypeName = "con bollettino";
                    break;
                case 2:
                    ViewBag.ProductTypeName = "semplice singolo destinatario";
                    break;
                case 3:
                    ViewBag.ProductTypeName = "con bollettino singolo destinatario";
                    break;
            }

            ViewBag.ProductTypeId = id;

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var senders = new List<Sender>();
            var loghi = new List<Loghi>();

            var userId = u.id;
            if (u.parentId > 0)
                userId = u.parentId;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "SendersUsers?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                senders = await get.Content.ReadAsAsync<List<Sender>>();

            get = await Globals.HttpClientSend("GET", "Loghi?userId=" + userId, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                loghi = await get.Content.ReadAsAsync<List<Loghi>>();

            var loghiSenders = new LoghiSenders()
            {
                senders = senders,
                loghi = loghi
            };

            return View(loghiSenders);
        }

        public async Task<ActionResult> GotoStep2(FormCollection dataForm)
        {

            if (Convert.ToInt32(dataForm["id"]) == (int)ProductType.singolo || Convert.ToInt32(dataForm["id"]) == (int)ProductType.bollettinoSingolo)
                return Redirect("DestinatarioSingolo?Bollettini=" + dataForm["id"] + "&Sender=" + dataForm["Sender"] + "&TipoStampa=" + dataForm["TipoStampa"] + "&FronteRetro=" + dataForm["FronteRetro"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"] + "&Formato=" + dataForm["Formato"] + "&logo=" + dataForm["logo"]);


            int SenderAR = 0;

            if (dataForm["RicevutaRitorno"] == "1")
            {
                //ESISTE LA RICEVUTA DI RITORNO
                //INSERISCO IL DESTINATARIO AR
                var NominativoMittenteAR = dataForm["nominativoSenderAR"];
                var CompletamentoNominativoMittenteAR = dataForm["complementoNominativoSenderAR"];
                var IndirizzoMittenteAR = dataForm["indirizzoSenderAR"];
                var CompletamentoIndirizzoMittenteAR = dataForm["completamentoIndirizzoSenderAR"];
                var CapMittenteAR = dataForm["capSenderAR"];
                var CittaMittenteAR = dataForm["cittaSenderAR"];
                var ProvinciaMittenteAR = dataForm["provinciaSenderAR"];
                var StatoMittenteAR = dataForm["statoSenderAR"];

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
                if (!cMAR.Valido)
                    return Redirect("/Home/Error500");


                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                HttpResponseMessage get = new HttpResponseMessage();

                //CREAZIONE MITTENTE AR
                sAR.temporary = true;
                sAR.userId = u.id;
                get = await Globals.HttpClientSend("POST", "/SendersUsers/New", u.areaTestUser, sAR);
                if (get.IsSuccessStatusCode)
                    SenderAR = await get.Content.ReadAsAsync<int>();

            }

            return Redirect("DestinatariNew?Bollettini=" + dataForm["id"] + "&Sender=" + dataForm["Sender"] + "&SenderAR=" + SenderAR + "&TipoStampa=" + dataForm["TipoStampa"] + "&FronteRetro=" + dataForm["FronteRetro"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"] + "&Formato=" + dataForm["Formato"] + "&logo=" + dataForm["logo"]);
        }

        public ActionResult Step2()
        {
            return View();
        }

        public async Task<ActionResult> GotoStep3(FormCollection dataForm)
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
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });

            UploadListResponse n = (UploadListResponse)Session["UploadResponse"];
            GetRecipentLists ln = new GetRecipentLists();
            n.NamesLists.ToList().ForEach(a => a.listId = id);
            ln.recipient = n.NamesLists.Where(a => a.valid == true).ToList();
            var lbulletins = new List<Bulletins>();
            if (n.Bulletins != null)
            {
                var i = 0;
                foreach (var b in n.NamesLists)
                {
                    if (b.valid == true)
                        lbulletins.Add(n.Bulletins[i]);

                    i++;

                }
                ln.bulletin = lbulletins;
            }

            get = await Globals.HttpClientSend("POST", "NamesLists/NewMultipleWithBulletin", u.areaTestUser, ln);
            if (!get.IsSuccessStatusCode)
            {
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Error500"
                });
            }

            ViewBag.listId = id;

            return Redirect("Step3?ListId=" + id + "&Bollettini=" + dataForm["Bollettini"] + "&Sender=" + dataForm["Sender"] + "&SenderAR=" + dataForm["SenderAR"] + "&TipoStampa=" + dataForm["TipoStampa"] + "&FronteRetro=" + dataForm["FronteRetro"] + "&RicevutaRitorno=" + dataForm["RicevutaRitorno"] + "&Formato=" + dataForm["Formato"]);
        }

        public async Task<ActionResult> Step3()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);
            ViewBag.guiduser = u.guidUser.ToString();
            ViewBag.userId = u.id;
            ViewBag.areaTestUser = u.areaTestUser;
            ViewBag.Mol = u.mol;

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

            return View(l);
        }

        public async Task<ActionResult> StepEnd(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/SetComplete?id=" + id + "&guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                Response.Redirect("/Home/Error500");

            ViewBag.guiduser = u.guidUser.ToString();

            var go = await get.Content.ReadAsAsync<GetOperations>();

            int logType = (int)LogType.sendRol;
            string description = "Richiesta nuova ROL id " + go.operationId + " confermata.";

            if (go.operationType == ((int)operationType.MOL).ToString())
            {
                logType = (int)LogType.sendMol;
                description = "Richiesta nuova MOL id " + go.operationId + " confermata.";
            };


            await Globals.SetLogs(logType, u.id, description);


            return View(go);
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

        public ActionResult DestinatariNew()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.userId = u.id;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.areaTestUser = u.areaTestUser;
            return View();
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

        public async Task<string> CalcolaPreventivo(FormCollection dataForm)
        {
            try
            {
                int ListId = Convert.ToInt32(dataForm["ListId"]);
                int Bollettini = Convert.ToInt32(dataForm["Bollettini"]);
                int Sender = Convert.ToInt32(dataForm["Sender"]);
                var SenderAR = dataForm["SenderAR"];
                int TipoStampa = Convert.ToInt32(dataForm["TipoStampa"]);
                int FronteRetro = Convert.ToInt32(dataForm["FronteRetro"]);
                int RicevutaRitorno = Convert.ToInt32(dataForm["RicevutaRitorno"]);
                int Formato = Convert.ToInt32(dataForm["Formato"]);
                string fileDir = dataForm["file"];
                string fileDirLoghi = Server.MapPath("/Upload/Loghi");

                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                HttpResponseMessage get = new HttpResponseMessage();

                //SENDER
                var s = new Sender();
                get = await Globals.HttpClientSend("GET", "SendersUsers/" + Sender, u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                    return "";

                s = await get.Content.ReadAsAsync<Sender>();

                //SENDER AR
                object sAR = null;
                if (SenderAR != "0") {
                    get = await Globals.HttpClientSend("GET", "SendersUsers/" + Convert.ToInt32(SenderAR), u.areaTestUser);
                    if (!get.IsSuccessStatusCode)
                        return "";

                    sAR = await get.Content.ReadAsAsync<Sender>();
                 }

                //RECIPIENTS
                var l = new GetLists();
                get = await Globals.HttpClientSend("GET", "Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                    return "";

                l = await get.Content.ReadAsAsync<GetLists>();

                List<GetRecipent> recipents = new List<GetRecipent>();
                //ATTACCHED FILE TO RECIPIENTS
                    foreach (var r in l.recipients)
                    {
                        if (System.IO.File.Exists(fileDir + "/" + r.fileName)) { 
                            GetRecipent gr = new GetRecipent();
                            r.fileName = fileDir + "/" + r.fileName;

                            //COVER
                            if (r.logo != "" && r.logo != null)
                            {
                                r.logo = fileDirLoghi + "/" + r.logo;
                            };

                            Names n = new Names();
                            n = Mapper.Map<Recipient, Names>(r);
                            gr.recipient = n;
                            recipents.Add(gr);
                         }
                    };

                if (recipents.Count == 0)
                    return "";

                var sr = new SenderRecipients()
                {
                    sender = s,
                    senderAR = (Sender)sAR,
                    recipients = recipents,
                    csvFile = Session["UploadFile"].ToString()
                };

                var autoConfirm = "";
                var url = "Rol/CheckAllFiles";

                if (u.mol)
                    url = "MOL/CheckAllFiles";

                 get = await Globals.HttpClientSend("POST", url + "?guidUser=" + u.guidUser.ToString() + "&tsc=" +
                    Convert.ToBoolean(TipoStampa)+ "&frc=" + Convert.ToBoolean(FronteRetro) + "&rrc=" + Convert.ToBoolean(RicevutaRitorno) + "&frm=" + Convert.ToBoolean(Formato) + "&userId=" + u.id, u.areaTestUser, sr);
                if (!get.IsSuccessStatusCode)
                    return get.StatusCode.ToString();

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

                Session["TotalNamesPrice"] = t;

                return new JavaScriptSerializer().Serialize(t);
            }
            catch (Exception e)
            {

                return "Scaduto il tempo massimo per l'operazione. Riprovare";
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<bool> Spedisci(FormCollection dataForm)
        {
            int operationId = Convert.ToInt32(dataForm["operationId"]);

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("GET", "/Rol/Confirm?guidUser=" + u.guidUser.ToString() + "&operationId=" + operationId, u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                Response.Redirect("/Home/Error500");

            return true;
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> UploadList(int bollettini, string logo)
        {
            try { 
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
                    //CONTROLLO CSV SE CONTIENE O NO BOLLETTINI
                    if (bollettini == 0)
                        res = await Globals.ReadCsvShowResults(dbDirectory + name, u, Session.SessionID, logo);
                    else
                        res = await Globals.ReadCsvBollettiniShowResults(dbDirectory + name, u, Session.SessionID, logo);

                        Session["UploadResponse"] = res;
                        Session["UploadFile"] = dbDirectory + name;
                        return new JavaScriptSerializer().Serialize(res);

                }
            }

            return new JavaScriptSerializer().Serialize(r);
            }
            catch(Exception e)
            {
                return e.Message.ToString();
            }
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
            HttpResponseMessage get = await Globals.HttpClientSend("POST", "/Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                id = await get.Content.ReadAsAsync<int>();
            if (id == 0)
                return "";

            get = await Globals.HttpClientSend("GET", "/NamesLists/ReplaceLists?ids=" + ids + "&listId=" + id, u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return "";

            return Convert.ToString(id);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> UploadFile(int ListId)
        {
            var list = new ListUploadFileRecipientResponse();
            var lur = new List<UploadFileRecipientResponse>();
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var f = Request.QueryString["fronteRetro"];
            bool fronteRetro = false;
            if (f == "0")
                fronteRetro = true;

            var files = Request.Files[0];

            string dir = "";

            if (files != null && files.ContentLength > 0)
            {
                var ex = Path.GetExtension(files.FileName).ToLower();
                if (ex == ".zip")
                {
                    var tick = DateTime.Now.Ticks;
                    var name = tick + ex;
                    var directory = Server.MapPath("/Upload/Users/" + u.id);

                    bool isExists = Directory.Exists(directory);
                    if (!isExists)
                        Directory.CreateDirectory(directory);

                    var specificDirectory = Server.MapPath("/Upload/Users/" + u.id + "/" + tick);

                    bool isExistsS = Directory.Exists(specificDirectory);
                    if (!isExistsS)
                        Directory.CreateDirectory(specificDirectory);

                    files.SaveAs(Path.Combine(specificDirectory + "/" + name));
                    Globals.ExtractFileZip(specificDirectory + "/" + name, specificDirectory);

                    dir = specificDirectory;

                    //RECIPIENTS
                    HttpResponseMessage get = new HttpResponseMessage();
                    var l = new GetLists();
                    get = await Globals.HttpClientSend("GET", "Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
                    if (!get.IsSuccessStatusCode)
                        return null;

                    l = await get.Content.ReadAsAsync<GetLists>();

                    foreach (var recipient in l.recipients)
                    {
                        var errorMessage = "";
                        var success = true;
                        if (!System.IO.File.Exists(dir + "/" + recipient.fileName))
                        {
                            errorMessage = "Nessun file corrispondente.";
                            success = false;
                        }
                        else
                        {
                            var c = Check.verificaDimensioneFile(dir + "/" + recipient.fileName, fronteRetro);
                            if (!c.Valido)
                            {
                                success = c.Valido;
                                errorMessage = c.Errore;
                            }
                        }

                        var ur = new UploadFileRecipientResponse() {
                            name = recipient,
                            errorMessage = errorMessage,
                            success = success
                        };
                        lur.Add(ur);
                    }

                    list.filePath = dir;
                    list.name = lur;
                }
            }

            return new JavaScriptSerializer().Serialize(list);
        }

        public string ValidateName(FormCollection fc)
        {
            var comuni = Globals.GetComuniList();

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
                fileName = fc["fileName"],
                complementAddress = fc["complementAddress"],
                complementNames = fc["complementNames"],
                fiscalCode = fc["fiscalCode"]
            };

            var comune = comuni.Where(a => a.cap == fc["cap"]);

            var c = new ControlloDestinatario();
            c = CheckRecipient.verificaDestinatario(s, comune, true);
            if(!c.Valido)
                return new JavaScriptSerializer().Serialize(c);

            if (Convert.ToInt32(fc["Bullettins"]) == 1)
            {
                Bulletins b = new Bulletins()
                {
                    NumeroContoCorrente = fc["NumeroContoCorrente"],
                    IntestatoA = fc["IntestatoA"],
                    CodiceCliente = fc["CodiceCliente"],
                    ImportoEuro = Convert.ToDecimal(fc["ImportoEuro"]),
                    EseguitoDaNominativo = fc["EseguitoDaNominativo"],
                    EseguitoDaIndirizzo = fc["EseguitoDaIndirizzo"],
                    EseguitoDaCAP = fc["EseguitoDaCAP"],
                    EseguitoDaLocalita = fc["EseguitoDaLocalita"],
                    Causale = fc["Causale"],
                    BulletinType = 2
                };

                var cb = new ControlloBollettino();
                cb = CheckBulletin.verificaBollettino(b);
                if(!cb.Valido)
                    return new JavaScriptSerializer().Serialize(cb);
            }
            return new JavaScriptSerializer().Serialize(c);
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
                fileName = fc["fileName"],
                complementAddress = fc["complementAddress"],
                complementNames = fc["complementNames"],
                fiscalCode = fc["fiscalCode"],
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


            List<Bulletins> lb = null;
            if (Convert.ToInt32(fc["Bullettins"]) == 1)
            {
                var bulletins = n.Bulletins.Select((c, i) => new { Bulletins = c, Index = i }).Where(x => x.Index == index);
                Bulletins b = new Bulletins()
                {
                    NumeroContoCorrente = fc["NumeroContoCorrente"],
                    IntestatoA = fc["IntestatoA"],
                    CodiceCliente = fc["CodiceCliente"],
                    ImportoEuro = Convert.ToDecimal(fc["ImportoEuro"]),
                    EseguitoDaNominativo = fc["EseguitoDaNominativo"],
                    EseguitoDaIndirizzo = fc["EseguitoDaIndirizzo"],
                    EseguitoDaCAP = fc["EseguitoDaCAP"],
                    EseguitoDaLocalita = fc["EseguitoDaLocalita"],
                    Causale = fc["Causale"],
                    IBAN = fc["IBAN"],
                    namesId = 0,
                    BulletinType = (int)bulletinType.Bollettino896,
                    id = 0
                };

                lb = new List<Bulletins>();

                var lbb = n.Bulletins.ToList();

                for (var y = 0; y < lbb.Count(); y++)
                {
                    if (y == index)
                        lb.Add(b);
                    else
                        lb.Add(lbb[y]);
                }

            }

            UploadListResponse nn = new UploadListResponse()
            {
                success = true,
                errorMessage = "",
                listId = 0,
                numberOfNames = n.numberOfNames,
                errors = (!name.valid ? n.errors - 1 : n.errors),
                NamesLists = ls,
                Bulletins = lb
            };

            Session["UploadResponse"] = nn;
            return new JavaScriptSerializer().Serialize(nn);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string SearchCity(FormCollection fc)
        {
            var cap = fc["cap"];
            var c = Globals.GetComuneFromCap(cap);

            return new JavaScriptSerializer().Serialize(c);
        }

        public async Task<string> ValidazioneMittenteDestinatario(FormCollection c)
        {
            var NominativoMittente = c["nominativoSender"];
            var CompletamentoNominativoMittente = c["complementoNominativoSender"];
            var IndirizzoMittente = c["indirizzoSender"];
            var CompletamentoIndirizzoMittente = c["completamentoIndirizzoSender"];
            var CapMittente = c["capSender"];
            var CittaMittente = c["cittaSender"];
            var ProvinciaMittente = c["provinciaSender"];
            var StatoMittente = c["statoSender"];

            var NominativoDestinatario = c["nominativoNames"];
            var ComplementoNominativoDestinatario = c["complementoNominativoNames"];
            var IndirizzoDestinatario = c["indirizzoNames"];
            var CompletamentoIndirizzoDestinatario = c["completamentoIndirizzoNames"];
            var CapDestinatario = c["capNames"];
            var CittaDestinatario = c["cittaNames"];
            var ProvinciaDestinatario = c["provinciaNames"];
            var StatoDestinatario = c["statoNames"];
            var NomeFile = c["fileName"];
            var selectedNamesListId = c["selectedNamesListId"];
            var namesId = c["namesId"];
            var logo = c["logo"];

            //SMS
            bool sendSms = false;
            string testoSms = null;
            string mobile = null;
            var sms = c["inviaSMS"];
            if (sms == "on")
            {
                testoSms = c["testoSmsNames"];
                mobile = c["mobileNames"];
                sendSms = true;
            }


                Sender s = new Sender()
            {
                businessName = NominativoMittente,
                complementNames = CompletamentoNominativoMittente,
                complementAddress = CompletamentoIndirizzoMittente,
                address = IndirizzoMittente,
                cap = CapMittente,
                city = CittaMittente,
                province = ProvinciaMittente,
                state = StatoMittente
            };

            var cM = new ControlloMittente();
            cM = CheckSender.verificaMittente(s);
            if (!cM.Valido)
               return new JavaScriptSerializer().Serialize(cM);

            //DESTINATARIO RICEVUTA DI RITORNO
            //DIVERSO DAL MITTENTE
            Sender sAR = null;
            var AR = c["addMittenteAR"];
            if(c["RicevutaRitorno"]!="0")
                if (AR == null)
                {
                    var NominativoMittenteAR = c["nominativoSenderAR"];
                    var CompletamentoNominativoMittenteAR = c["complementoNominativoSenderAR"];
                    var IndirizzoMittenteAR = c["indirizzoSenderAR"];
                    var CompletamentoIndirizzoMittenteAR = c["completamentoIndirizzoSenderAR"];
                    var CapMittenteAR = c["capSenderAR"];
                    var CittaMittenteAR = c["cittaSenderAR"];
                    var ProvinciaMittenteAR = c["provinciaSenderAR"];
                    var StatoMittenteAR = c["statoSenderAR"];

                    sAR = new Sender()
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
                    if (!cMAR.Valido)
                        return new JavaScriptSerializer().Serialize(cMAR);
                };

            var comuni = Globals.GetComuniList();

            NamesLists nl = new NamesLists()
            {
                businessName = NominativoDestinatario,
                complementNames = ComplementoNominativoDestinatario,
                address = IndirizzoDestinatario,
                complementAddress = CompletamentoIndirizzoDestinatario,
                cap = CapDestinatario == "" ? "0" : CapDestinatario,
                city = CittaDestinatario,
                province = ProvinciaDestinatario == "" ? "EE" : ProvinciaDestinatario,
                state = StatoDestinatario,
                fileName = NomeFile,
                sms = sendSms,
                testoSms = testoSms,
                mobile = mobile,
                logo = logo
            };

            var comune = comuni.Where(a => a.cap == CapDestinatario);

            var cD = new ControlloDestinatario();
            cD = CheckRecipient.verificaDestinatario(nl, comune, true);
            if(!cD.Valido)
                return new JavaScriptSerializer().Serialize(cD);

            Bulletins b = new Bulletins();
            if (Convert.ToInt32(c["Bollettini"]) == 3)
            {
                b = new Bulletins()
                {
                    NumeroContoCorrente = c["NumeroContoCorrente"],
                    IntestatoA = c["IntestatoA"],
                    CodiceCliente = c["CodiceCliente"],
                    ImportoEuro = Convert.ToDecimal(c["ImportoEuro"]),
                    EseguitoDaNominativo = c["EseguitoDaNominativo"],
                    EseguitoDaIndirizzo = c["EseguitoDaIndirizzo"],
                    EseguitoDaCAP = c["EseguitoDaCAP"],
                    EseguitoDaLocalita = c["EseguitoDaLocalita"],
                    Causale = c["Causale"],
                    BulletinType = 2
                };

                var cb = new ControlloBollettino();
                cb = CheckBulletin.verificaBollettino(b);
                if (!cb.Valido)
                    return new JavaScriptSerializer().Serialize(cb);

            }

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
            get = await Globals.HttpClientSend("POST", "SendersUsers/New", u.areaTestUser, s);
            if (get.IsSuccessStatusCode)
                Sender = await get.Content.ReadAsAsync<int>();


            if (c["RicevutaRitorno"] != "0")
                if (AR == null)
                {
                    sAR.temporary = true;
                    sAR.userId = u.id;
                    get = await Globals.HttpClientSend("POST", "SendersUsers/New", u.areaTestUser, sAR);
                    if (get.IsSuccessStatusCode)
                        SenderAR = await get.Content.ReadAsAsync<int>();
                }

            int listId = 0;
            if (Convert.ToInt32(selectedNamesListId)> 0)
                nl.noUse = true;

            //CREAZIONE DESTINATARIO
            //LISTA PROVVISORIA PER INSERIMENTO DESTINATARIO
            var l = new Lists();
            l.name = "Lista provvisoria singolo destinatario";
            l.description = "Lista in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
            l.userId = u.id;
            l.date = DateTime.Now;
            l.temporary = true;

            get = await Globals.HttpClientSend("POST", "Lists/New", u.areaTestUser, l);
            if (get.IsSuccessStatusCode)
                listId = await get.Content.ReadAsAsync<int>();


            //INSERIMENTO DESTINATARIO ED EVENTUALE BOLLETTINO
            GetRecipentLists ln = new GetRecipentLists();
            nl.listId = listId;
            nl.fileName = c["fileName"];

            var r = new List<NamesLists>();
            r.Add(nl);
            ln.recipient = r;

            List<Bulletins> bl = new List<Bulletins>();
            string bollettini = "0";
            if (Convert.ToInt32(c["Bollettini"]) == 3)
            {
                bollettini = "1";
                bl.Add(b);
                ln.bulletin = bl;
            }

            get = await Globals.HttpClientSend("POST", "NamesLists/NewMultipleWithBulletin", u.areaTestUser, ln);

            var rr = new RedirectSingleNames()
            {
                ListId = listId,
                Sender = Sender,
                SenderAR = SenderAR,
                Bollettini = bollettini,
                TipoStampa = c["TipoStampa"],
                FronteRetro = c["FronteRetro"],
                RicevutaRitorno = c["RicevutaRitorno"],
                Formato = c["Formato"],
                Valido = true
            };

            return new JavaScriptSerializer().Serialize(rr);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string UploadFileSingolo(FormCollection dataForm)
        {
            var r = new UploadFileResponse();

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var f = Request.QueryString["fronteRetro"];
            bool fronteRetro = false;
            if (f == "0")
                fronteRetro = true;

            var file = Request.Files[0];

            if (file != null && file.ContentLength > 0)
            {
                var ex = Path.GetExtension(file.FileName);
                if (ex == ".pdf")
                {
                    var name = DateTime.Now.Ticks + ex;
                    var directory = Server.MapPath("/Upload/FilePdf/");
                    var dbDirectory = Globals.staticUrl + "Upload/FilePdf/";
                    file.SaveAs(Path.Combine(directory + name));

                    var c = Check.verificaDimensioneFile(Path.Combine(directory + name), fronteRetro);

                    if(!c.Valido) {
                        r.success = false;
                        r.fileName = "";
                        r.errorMessage = c.Errore;
                        return new JavaScriptSerializer().Serialize(r);
                    }

                    r.success = true;
                    r.fileName = name;
                    return new JavaScriptSerializer().Serialize(r);

                }
            }

            r.success = false;
            r.errorMessage = "Errore nel caricamento del file";
            r.fileName = "";

            return new JavaScriptSerializer().Serialize(r);
        }

        public async Task<ActionResult> GeneraPreventivoSingoloDestinatario()
        {
            try
            {
                int ListId = Convert.ToInt32(Request.QueryString["ListId"]);
                int Bollettini = Convert.ToInt32(Request.QueryString["Bollettini"]);
                int Sender = Convert.ToInt32(Request.QueryString["Sender"]);
                int SenderAR = Convert.ToInt32(Request.QueryString["SenderAR"]);
                int TipoStampa = Convert.ToInt32(Request.QueryString["TipoStampa"]);
                int FronteRetro = Convert.ToInt32(Request.QueryString["FronteRetro"]);
                int RicevutaRitorno = Convert.ToInt32(Request.QueryString["RicevutaRitorno"]);
                int Formato = Convert.ToInt32(Request.QueryString["Formato"]);
                string fileDir = Server.MapPath("/Upload/FilePdf");
                string fileDirLoghi = Server.MapPath("/Upload/Loghi");

                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                HttpResponseMessage get = new HttpResponseMessage();

                //SENDER
                var s = new Sender();
                get = await Globals.HttpClientSend("GET", "SendersUsers/" + Sender, u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");

                s = await get.Content.ReadAsAsync<Sender>();

                //SENDERAR
                object sAR = null;
                if (SenderAR > 0)
                {
                    get = await Globals.HttpClientSend("GET", "SendersUsers/" + SenderAR, u.areaTestUser);
                    if (!get.IsSuccessStatusCode)
                        return Redirect("/Home/Error500");

                    sAR = await get.Content.ReadAsAsync<Sender>();

                }

                //RECIPIENTS
                var l = new GetLists();
                get = await Globals.HttpClientSend("GET", "Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");

                l = await get.Content.ReadAsAsync<GetLists>();

                List<GetRecipent> recipents = new List<GetRecipent>();
                //ATTACCHED FILE TO RECIPIENTS
                foreach (var r in l.recipients)
                {
                    GetRecipent gr = new GetRecipent();
                    r.fileName = fileDir + "/" + r.fileName;
                    
                    //COVER
                    if(r.logo != "" && r.logo != null)
                    {
                        r.logo = fileDirLoghi + "/" + r.logo;
                    };

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

                var j = JsonConvert.SerializeObject(sr);

                var url = "Rol/CheckAllFiles";

                if (u.mol)
                    url = "MOL/CheckAllFiles";

                get = await Globals.HttpClientSend("POST", url + "?guidUser=" + u.guidUser.ToString() + "&tsc=" +
                   Convert.ToBoolean(TipoStampa) + "&frc=" + Convert.ToBoolean(FronteRetro) + "&rrc=" + Convert.ToBoolean(RicevutaRitorno) + "&frm=" + Convert.ToBoolean(Formato) + "&userId=" + u.id, u.areaTestUser, sr);
                if (!get.IsSuccessStatusCode)
                    return Redirect("/Home/Error500");

                GetNumberOfCheckedNames gor = await get.Content.ReadAsAsync<GetNumberOfCheckedNames>();
                decimal importoNetto = 0;
                decimal importoIva = 0;
                decimal importoTotale = 0;

                if (!u.mol) { 
                    foreach (var sres in gor.checkedNames)
                    {
                        importoNetto += sres.price.price;
                        importoIva += sres.price.vatPrice;
                        importoTotale += sres.price.totalPrice;
                    }
                }
                
                var t = new TotalNamesPrice()
                {
                    importoTotale = Math.Round(importoTotale, 2),
                    importoIva = Math.Round(importoIva, 2),
                    importoNetto = Math.Round(importoNetto, 2),
                    numberOfNames = gor.numberOfValidNames,
                    operationId = gor.operationId
                };

                Session["TotalNamesPrice"] = t;

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
            var t = new TotalNamesPrice();
            t =  (TotalNamesPrice)Session["TotalNamesPrice"];
 
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            HttpResponseMessage get = new HttpResponseMessage();
            var l = new GetOperations();
            get = await Globals.HttpClientSend("GET", "Operations/" + t.operationId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
            if (!get.IsSuccessStatusCode)
                return Redirect("/Home/Error500");

            int logType = (int)LogType.sendRol;
            string description = "Richiesta nuova ROL id " + t.operationId + " da confermare.";

            if (l.operationType == ((int)operationType.MOL).ToString()) {
                logType = (int)LogType.sendMol;
                description = "Richiesta nuova MOL id " + t.operationId + " da confermare.";
            };


            await Globals.SetLogs(logType, u.id, description);

            l = await get.Content.ReadAsAsync<GetOperations>();

            ViewBag.fronteRetro = l.recipients[0].fronteRetro;
            ViewBag.tipoStampa = l.recipients[0].tipoStampa;
            ViewBag.ricevutaRitorno = l.recipients[0].ricevutaRitorno;

            return View(t);
        }
    }

}