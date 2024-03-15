using AutoMapper;
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
using static iTextSharp.text.pdf.PRTokeniser;

namespace WasySender_2_0.Controllers
{
    public class PacchiController : Controller
    {
        // GET: Pacchi
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
            return Redirect("DestinatariNew?Sender=" + dataForm["Sender"] + "&Prodotto=" + dataForm["Prodotto"] + "&MittenteDaContratto=" + dataForm["MittenteDaContratto"]);
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

        public ActionResult Riepilogo()
        {
            var t = new TotalNamesPrice();
            t = (TotalNamesPrice)Session["TotalNamesPricePacchi"];


            return View(t);
        }

        public ActionResult Preventivo()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.userId = u.id;
            ViewBag.sessionId = Session.SessionID;
            ViewBag.areaTestUser = u.areaTestUser;
            return View();
        }

        public async Task<ActionResult> Invii()
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

            var filter = "&operationType=7";
            if (u.parentId > 0)
                filter = "&userId=" + u.id;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations?guidUser=" + u.guidUser + filter, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<List<GetOperations>>();

            ViewBag.productType = "PACCHI";
            GetOperations = GetOperations.ToList();

            await Globals.SetLogs((int)LogType.visPgk, u.id, "Visualizzazione invio pacchi");


            return View(GetOperations);
        }

        public async Task<ActionResult> DettaglioInvio(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.guiduser = u.guidUser;
            ViewBag.userName = (u.businessName != "" ? u.businessName : u.name + " " + u.lastName);

            var GetOperationsNew = new GetOperationsNew();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/Items/" + id + "?guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperationsNew = await get.Content.ReadAsAsync<GetOperationsNew>();

            return View(GetOperationsNew);
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

            return View(go);
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> UploadList(string senderFromContract, string product)
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
                        bool sf = false;
                        if (senderFromContract == "1")
                            sf = true;
                        res = await Globals.ReadCsvShowResultsPacchi(dbDirectory + name, u, Session.SessionID, sf, product);

                        Session["UploadResponsePacchi"] = res;
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



        public async Task<ActionResult> GeneraPreventivo(FormCollection dataForm)
        {
            try
            {
                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                int ListId = 0;
                int Sender = Convert.ToInt32(dataForm["Sender"]);


                var l = new Lists();
                l.name = "Lista caricamento";
                l.description = "Caricamento file in data : " + DateTime.Now.ToString("dd/MM/yyyy") + ", alle ore : " + DateTime.Now.ToString("HH:mm:ss");
                l.userId = u.id;
                l.date = DateTime.Now;

                HttpResponseMessage get = await Globals.HttpClientSend("POST", "Lists/New", u.areaTestUser, l);
                if (get.IsSuccessStatusCode)
                    ListId = await get.Content.ReadAsAsync<int>();

                if (ListId == 0)
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });

                UploadListResponse n = (UploadListResponse)Session["UploadResponsePacchi"];
                GetRecipentLists ln = new GetRecipentLists();
                n.NamesLists.ToList().ForEach(a => a.listId = ListId);
                ln.recipient = n.NamesLists.Where(a => a.valid == true).ToList();

                get = await Globals.HttpClientSend("POST", "NamesLists/NewMultipleWithBulletin", u.areaTestUser, ln);
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
                get = await Globals.HttpClientSend("GET", "SendersUsers/" + Sender, u.areaTestUser);
                if (!get.IsSuccessStatusCode)
                {
                    return RedirectToRoute(new
                    {
                        controller = "Home",
                        action = "Error500"
                    });
                }
                s = await get.Content.ReadAsAsync<Sender>();


                //RECIPIENTS
                var gl = new GetLists();
                get = await Globals.HttpClientSend("GET", "Lists/" + ListId + "?guidUser=" + u.guidUser.ToString(), u.areaTestUser);
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
                    r.fileName = null;
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
                    senderAR = null,
                    recipients = recipents
                };

                var url = "Pacchi/CheckAllFiles";

                get = await Globals.HttpClientSend("POST", url + "?guidUser=" + u.guidUser.ToString() + "&userId=" + u.id, u.areaTestUser, sr);
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

                Session["TotalNamesPricePacchi"] = t;

                return View("Preventivo");
            }
            catch (Exception e)
            {
                return Redirect("/Home/Error500");
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public string GetNames(FormCollection fc)
        {
            int index = Convert.ToInt32(fc["index"]);

            UploadListResponse n = (UploadListResponse)Session["UploadResponsePacchi"];

            var nb = new List<object>();
            var namelist = n.NamesLists.Select((c, i) => new { NamesLists = c, Index = i }).Where(x => x.Index == index);
            var ne = namelist.FirstOrDefault();
            NamesLists name = ne.NamesLists;
            name.fiscalCode = Convert.ToDateTime(name.shipmentDate).ToString("dd/MM/yyyy");
            nb.Add(name);

            var j = new JavaScriptSerializer().Serialize(nb);

            return j;
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
                fiscalCode = fc["fiscalCode"],
                shipmentDate = Convert.ToDateTime(fc["shipmentDate"]),
                weight = Convert.ToInt32(fc["weight"]),
                height = Convert.ToInt32(fc["height"]),
                length = Convert.ToInt32(fc["length"]),
                width = Convert.ToInt32(fc["width"]),
                contentText = fc["contentText"],
                additionalServices = fc["additionalServices"]
            };

            var comune = comuni.Where(a => a.cap == fc["cap"]);

            var c = new ControlloDestinatario();
            c = CheckRecipient.verificaDestinatarioPacchi(s, comune, true);
            if (!c.Valido)
                return new JavaScriptSerializer().Serialize(c);

            return new JavaScriptSerializer().Serialize(c);
        }


        [HttpPost]
        [ValidateInput(false)]
        public string SaveNameList(FormCollection fc)
        {
            int index = Convert.ToInt32(fc["index"]);

            UploadListResponse n = (UploadListResponse)Session["UploadResponsePacchi"];
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
                shipmentDate = Convert.ToDateTime(fc["shipmentDate"]),
                weight = Convert.ToInt32(fc["weight"]),
                height = Convert.ToInt32(fc["height"]),
                length = Convert.ToInt32(fc["length"]),
                width = Convert.ToInt32(fc["width"]),
                contentText = fc["contentText"],
                additionalServices = Globals.GeneraAdditionalServices(fc["contrassegno"], fc["ritornoAlMittente"], fc["assicurazione"]),
                contrassegno = fc["contrassegno"],
                ritornoAlMittente = fc["ritornoAlMittente"],
                assicurazione = fc["assicurazione"],
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

            Session["UploadResponsePacchi"] = nn;
            return new JavaScriptSerializer().Serialize(nn);
        }


        public async Task<ActionResult> ArchivioSpedizioni()
        {
            var dataDa = Request.QueryString["dataDa"];
            var dataA = Request.QueryString["dataA"];
            var nominativo = Request.QueryString["nominativo"];
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            if (!u.changePwd)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "ChangePwd"
                });

            ViewBag.guiduser = u.guidUser;

            var start = "";
            var page = Request.QueryString["page"];
            if (page != null)
                start = "&start=" + (Convert.ToInt32(page) - 1);


            int pageSize = 20;

            var GetOperations = new TotalOperations();

            var filter = "&completeTransmission=true&pageSize=" + pageSize;
            if (u.parentId > 0)
                filter += "&userId=" + u.id;

            if (dataDa != null && dataDa != "")
                filter += "&startDate=" + dataDa;

            if (dataA != null && dataA != "")
                filter += "&endDate=" + dataA;

            if (nominativo != null && nominativo != "")
                filter += "&reciverName=" + nominativo;

            filter += "&operationType=7";

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/Total?guidUser=" + u.guidUser + filter + start, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<TotalOperations>();

            if (GetOperations.count > 0)
            {
                decimal p = Decimal.Divide(GetOperations.count, pageSize);
                ViewBag.pageNumber = Convert.ToInt32(Math.Ceiling(p));
            }
            else
                ViewBag.pageNumber = 1;

            ViewBag.selectedPage = page == null ? "1" : page;

            int logType = (int)LogType.requestArchive;
            string description = "Visualizzazione Archivio spedizioni pacchi pagina " + ViewBag.selectedPage;

            await Globals.SetLogs(logType, u.id, description);


            return View(GetOperations);
        }


    }
}