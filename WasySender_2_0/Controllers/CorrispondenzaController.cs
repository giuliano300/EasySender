using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WasySender_2_0.DataModel;
using WasySender_2_0.Models;

namespace WasySender_2_0.Controllers
{
    public class CorrispondenzaController : Controller
    {
        // GET: Corrispondenza
        public async Task<ActionResult> Index(int id)
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

            var filter = "";
            if (u.parentId > 0)
                filter = "&userId=" + u.id;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations?guidUser=" + u.guidUser + filter, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<List<GetOperations>>();

            if (u.mol) {
                if (id == (int)operationType.ROL)
                    id = (int)operationType.MOL;
            }

            if (u.col)
            {
                if (id == (int)operationType.LOL)
                    id = (int)operationType.COL;
            }

            int logType = 0;
            switch (id)
            {
                case (int)operationType.ROL:
                    logType = (int)LogType.visRol;
                    ViewBag.productType = "RACCOMANDATE";
                    break;
                case (int)operationType.LOL:
                    logType = (int)LogType.visLol;
                    ViewBag.productType = "LETTERE";
                    break;
                case (int)operationType.TELEGRAMMA:
                    logType = (int)LogType.visTol;
                    ViewBag.productType = "TELEGRAMMI";
                    break;
                case (int)operationType.COL:
                    logType = (int)LogType.visCol;
                    ViewBag.productType = "POSTA CONTEST";
                    break;
                case (int)operationType.MOL:
                    logType = (int)LogType.visMol;
                    ViewBag.productType = "MARKET";
                    break;
            }

            GetOperations = GetOperations.Where(a => a.type == id).ToList();

            await Globals.SetLogs(logType, u.id, "Visualizzazione corrispondenza");

            return View(GetOperations);
        }

        public async Task<ActionResult> Report()
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
            string description = "Visualizzazione Archivio spedizioni senza bollettini";

            await Globals.SetLogs(logType, u.id, description);
            return View();
        }

        public async Task<ActionResult> ErroriNotificati()
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

            int logType = (int)LogType.visErrors;
            string description = "Visualizzazione errori notificati";

            await Globals.SetLogs(logType, u.id, description);

            return View();
        }

        public async Task<ActionResult> ConciliazionePagamenti()
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
            string description = "Visualizzazione Archivio spedizioni con bollettini";

            await Globals.SetLogs(logType, u.id, description);


            return View();
        }

        public ActionResult ErroriLotto()
        {
            return View();
        }

        public async Task<ActionResult> DettaglioLotto(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            ViewBag.guiduser = u.guidUser;
            ViewBag.userName = (u.businessName != "" ? u.businessName : u.name + " " + u.lastName);

            var GetOperationsNew = new GetOperationsNew();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations/Items/" + id + "?guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperationsNew = await get.Content.ReadAsAsync<GetOperationsNew>();

            int logType = (int)LogType.requestArchive;
            string description = "Visualizzazione dettaglio spedizione id " + id;

            await Globals.SetLogs(logType, u.id, description);

            return View(GetOperationsNew);
        }

        public ActionResult ProdottiPostali()
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

        public async Task<ActionResult> ExportPDF(int id)
        {
            if (Request.Cookies["login"] == null)
                return RedirectToRoute(new
                {
                    controller = "Home",
                    action = "Index"
                });

            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);


            int logType = (int)LogType.requestArchive;
            string description = "Download distina spedizione id " + id;

            await Globals.SetLogs(logType, u.id, description);

            return new ActionAsPdf("Distinta", new { id = id , guidUser = u.guidUser.ToString(), areaTest = u.areaTestUser})
            {
                FileName = Server.MapPath("~/Download/distinta.pdf"),
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.A4
            };
        }

        public async Task<string> GetZIP(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var res = new ZipResponse();

            var GetOperations = new GetOperations();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations/" + id + "?guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<GetOperations>();

            var filePath = "/Public/ZipFile/file-" + DateTime.Now.Ticks.ToString() + ".zip";
            res.pathFile = filePath;

            var zipFile = HostingEnvironment.MapPath(filePath);
            try 
            { 
                using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                {
                    foreach (var fPath in GetOperations.recipients.Select(a=>a.pathRecoveryFile))
                    {
                        if (fPath != null) 
                        {
                            var fileName =  "/" + fPath.Split('/')[3];
                            var urlFile = Globals.downladFile + fileName;
                            WebClient webClient = new WebClient();
                            webClient.DownloadFile(urlFile, HostingEnvironment.MapPath("/Public/ZipFile/") + fileName);
                            archive.CreateEntryFromFile(HostingEnvironment.MapPath("/Public/ZipFile/") + fileName, Path.GetFileName(fPath));
                            System.IO.File.Delete(Path.Combine(HostingEnvironment.MapPath("/Public/ZipFile/") + fileName));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                res.success = false;
                res.pathFile = "";
            }

            return JsonConvert.SerializeObject(res);
        }

        public async Task<string> GetZIPRR(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var res = new ZipResponse();

            var GetOperations = new GetOperations();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/" + id + "?guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<GetOperations>();

            var filePath = "/Public/ZipFile/file-RR-" + DateTime.Now.Ticks.ToString() + ".zip";
            res.pathFile = filePath;

            var zipFile = HostingEnvironment.MapPath(filePath);
            try
            {
                using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                {
                    foreach (var fPath in GetOperations.recipients.Select(a => a.pathGEDUrl))
                    {
                        if (fPath != null)
                        {
                            var fileName = "/" + fPath;
                            var urlFile = Globals.downladFile + fileName;
                            WebClient webClient = new WebClient();
                            webClient.DownloadFile(urlFile, HostingEnvironment.MapPath("/Public/ZipFile/") + fileName);
                            archive.CreateEntryFromFile(HostingEnvironment.MapPath("/Public/ZipFile/") + fileName, Path.GetFileName(fPath));
                            System.IO.File.Delete(Path.Combine(HostingEnvironment.MapPath("/Public/ZipFile/") + fileName));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                res.success = false;
                res.pathFile = "";
            }

            return JsonConvert.SerializeObject(res);
        }

        public async Task<ActionResult> Distinta(int id, string guidUser, bool areaTest)
        {

            var GetOperations = new GetOperations();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Operations/" + id + "?guidUser=" + guidUser, areaTest);
            if (get.IsSuccessStatusCode)
                GetOperations = await get.Content.ReadAsAsync<GetOperations>();

            return View(GetOperations);
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


            string description = "Visualizzazione archivio spedizione pagina " + ViewBag.selectedPage;

            int logType = (int)LogType.requestArchive;
            await Globals.SetLogs(logType, u.id, description);

            return View(GetOperations);
        }

        public ActionResult DettaglioSpedizione()
        {
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
            if (df["type"] != null)
                stringConcat += "&prodotto=" + df["type"];

            var lgo = new List<GetOperationNames>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/GetAllOperationsNewNoBulletins?guidUser=" + u.guidUser + stringConcat, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                lgo = await get.Content.ReadAsAsync<List<GetOperationNames>>();


            string csv = "TIPO;MITTENTE;NOMINATIVO;COMPLETAMENTO NOMINATIVO;ESITO;DATA ACCETTAZIONE;PREZZO;FILE;CODICE;STATO;DATA DI CONSEGNA;MOTIVAZIONE\n";
            foreach (var go in lgo)
            {
                var r = go.recipient.recipient;
                var name = r.fileName.Split('\\');
                csv += go.operationType + ";" + go.sender.businessName + " " + go.sender.name +  " " + go.sender.surname + "; " + r.businessName + " " + r.name + " " + r.surname + ";" + r.complementNames + ";" + (r.orderId != "" ? "OK" : "ERRORE") + ";" + r.presaInCaricoDate + ";" + r.totalPrice + ";" + name[name.Length - 1].Split('/')[name[name.Length - 1].Split('/').Length - 1] + ";" + r.codice + ";" + (r.stato != null ? r.stato : "Stato non disponibile") + ";" + (r.consegnatoDate == null ? "" : Convert.ToDateTime(r.consegnatoDate).ToString("dd/MM/yyyy")) + ";" + r.stato + "\n";
            }

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report-" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv");
        }

        public async Task<FileContentResult> DownloadCsvBulletin(FormCollection df)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var stringConcat = "&userId=" + u.id + "&bollettini=true";
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
            if (df["type"] != null)
                stringConcat += "&prodotto=" + df["type"];
            if (df["dataDaPayments"] != null)
                stringConcat += "&dataDaPayments=" + df["dataDaPayments"];
            if (df["dataAPayments"] != null)
                stringConcat += "&dataAPayments=" + df["dataAPayments"];
            if (df["pagato"] != null)
                stringConcat += "&pagato=" + df["pagato"];

            var lgo = new List<GetOperationNames>();

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "Operations/GetAllOperationsNewWithBulletins?guidUser=" + u.guidUser + stringConcat, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                lgo = await get.Content.ReadAsAsync<List<GetOperationNames>>();


            string csv = "TIPO;MITTENTE;NOMINATIVO;CODICE CLIENTE;COMPLETAMENTO NOMINATIVO;ESITO;DATA ACCETTAZIONE;PREZZO;FILE;CODICE;STATO\n";
            foreach (var go in lgo)
            {
                var r = go.recipient.recipient;
                var name = r.fileName.Split('\\');
                csv += go.operationType + ";" + go.sender.businessName + " " + go.sender.name + " " + go.sender.surname + "; " + r.businessName + " " + r.name + " " + r.surname + ";" + go.recipient.bulletin.CodiceCliente + ";" + r.complementNames + ";" + (r.orderId != "" ? "OK" : "ERRORE") + ";" + r.presaInCaricoDate + ";" + r.totalPrice + ";" + name[name.Length - 1].Split('/')[name[name.Length - 1].Split('/').Length - 1] + ";" + r.codice + "; " + (r.stato != null ? r.stato : "Stato non disponibile") + "\n";
            }

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Report-" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv");
        }
        public async Task<FileContentResult> CreateErrorFile()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var stringConcat = "";

            var res = (UploadListResponse)Session["UploadResponse"];

            var errorList = res.NamesLists.Where(a=>a.valid == false);

            string csv = "RagioneSociale;Nome;Cognome;CAP;Citta;Provincia;Stato;Indirizzo;CompletamentoIndirizzo;CompletamentoNominativo;NomeFile;CodiceFiscale;Telefono;Errore\n";
            foreach (var e in errorList)
            {
               csv += e.businessName + ";" + e.name  + ";" + e.surname  + ";" + e.cap + ";" + e.city + ";" + e.province + ";" + e.state + ";" + e.address + ";" + e.complementAddress + ";" + e.complementNames + ";" + e.fileName + ";" + e.fiscalCode + ";" + e.mobile + ";" + e.errorMessage + "\n";
            }

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", "Errors-" + DateTime.Now.Ticks.ToString() + ".csv");
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

        public async Task<string> GetPdfGED(int id)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var GetOperations = new GetOperations();
            var fileName = "";

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Names/GetFileGED?nameId=" + id + "&guidUser=" + u.guidUser, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                fileName = await get.Content.ReadAsAsync<string>();

            if (fileName == "")
                return fileName;

            var url = Globals.downladFile;

            return url + fileName;
        }

    }
}