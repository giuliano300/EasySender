using BackOffice.Models;
using BackOffice.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BackOffice.Controllers
{
    public class CounterController : Controller
    {
        // GET: Counter
        public ActionResult Index()
        {
            return View();
        }
        // GET: Counter
        public ActionResult Invii()
        {
            return View();
        }
        public ActionResult ReportTot()
        {
            return View();
        }
        // GET: Counter
        public async Task<ActionResult> Mod(int id)
        {
            GetNamesComplete c = new GetNamesComplete();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Backoffice/GetNamesComplete?id=" + id);
            if (get.IsSuccessStatusCode)
                c = await get.Content.ReadAsAsync<GetNamesComplete>();

            if (c.name.ricevutaRitorno == true && c.senderAR == null)
                c.senderAR = c.sender;

            return View(c);
        }

        public async Task<string> GetPdf(int id)
        {
            var fileName = "";

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Backoffice/GetFile?nameId=" + id, false);
            if (get.IsSuccessStatusCode)
                fileName = await get.Content.ReadAsAsync<string>();

            if (fileName == "")
                return fileName;

            var url = Globals.downladFile;

            return url + fileName.Split('/')[3];
        }

        public async Task<string> GetPdfOld(FormCollection c)
        {
            var url = Globals.staticUrl;
            var index = c["file"].IndexOf("UploadFilePdf");
            if (index >= 0)
                url += "UploadFile/Pdf/" + c["file"].Split('/')[1];
            else 
            {
                var doc = c["file"].Split('Î')[1];
                var userId = doc.Substring(0, 3);
                var newDoc = doc.Replace(userId, "");
                url += "Upload/Users/" + userId + "/" + newDoc;
            }

            return url;
        }

        public async Task<bool> NotificaInvio(int id)
        {
            var ok = false;

            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Backoffice/NotificaNames?nameId=" + id, false);
            if (get.IsSuccessStatusCode)
                ok = await get.Content.ReadAsAsync<bool>();


            return ok;
        }

        public async Task<bool> BulkSaveSend(FormCollection f)
        {
            var data = f["d"].Split(',');
            for(var x = 0; x < data.Length; x++)
            {
                GetNamesComplete c = new GetNamesComplete();
                HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Backoffice/GetNamesComplete?id=" + data[x]);
                if (get.IsSuccessStatusCode)
                    c = await get.Content.ReadAsAsync<GetNamesComplete>();

                var m = new ModAndSendNames()
                {
                    businessName = c.name.businessName,
                    name = c.name.name,
                    surname = c.name.surname,
                    address = c.name.address,
                    complementNames = c.name.complementNames,
                    complementAddress = c.name.complementAddress,
                    city =c.name.city,
                    province = c.name.province, 
                    cap = c.name.cap,
                    state = c.name.state,
                    id = c.name.id,
                    sender = c.sender,
                    senderAR = c.senderAR,
                    fr = (bool)c.name.fronteRetro,
                    rr = (bool)c.name.ricevutaRitorno,
                    bn = (bool)c.name.tipoStampa,
                    tipoLettera = c.name.tipoLettera
                };

                var t = await Globals.HttpClientSend("POST", "/api/Backoffice/ModAndSend", m);

            }
            return true;
        }

        public async Task<bool> BulkAssegnaCodiceMOLCOL(FormCollection f)
        {
            string ids = f["d"];
            await Globals.HttpClientSend("POST", "/api/Backoffice/AssegnaCodiceMOLCOL?nId=" + ids);
            return true;
        }

        public async Task<bool> BulkSignSend(FormCollection f)
        {
            string ids = f["d"];
            await Globals.HttpClientSend("GET", "/api/Backoffice/BulkSignSend?ids=" + ids);
            return true;
        }

        public async Task<ActionResult> SaveSend(FormCollection f)
        {
            var segnaSpedito = f["segnaSpedito"];
            if (segnaSpedito == "1")
                await Globals.HttpClientSend("GET", "/api/Backoffice/SignSend?id=" + f["id"]);
            else
            {
                //MODIFICA VALORI MITTENTE
                var s = new Sender();
                HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/Sender/" + f["senderId"]);
                if (get.IsSuccessStatusCode)
                    s = await get.Content.ReadAsAsync<Sender>();

                s.businessName = f["senderBusinessName"];
                s.address = f["senderAddress"];
                s.cap = f["senderCap"];
                s.city = f["senderCity"];
                s.state = f["senderState"];


                //MODIFICA VALORI MITTENTE AR
                var sAR = new Sender();
                if (f["senderARId"] != null) { 
                    get = await Globals.HttpClientSend("GET", "/api/Sender/" + f["senderARId"]);
                    if (get.IsSuccessStatusCode)
                        sAR = await get.Content.ReadAsAsync<Sender>();

                    sAR.businessName = f["senderARBusinessName"];
                    sAR.address = f["senderARAddress"];
                    sAR.cap = f["senderARCap"];
                    sAR.city = f["senderARCity"];
                    sAR.state = f["senderARState"];
                }


                var rr = f["rr"];
                var ts = f["ts"];
                var fr = f["fr"];


                bool ricevutaRitorno = true;
                if (rr == null)
                    ricevutaRitorno = false;

                bool tipoStampa = true;
                if (ts == null)
                    tipoStampa = false;

                bool fronteRetro = true;
                if (fr == null)
                    fronteRetro = false;


                var m = new ModAndSendNames()
                {
                    businessName = f["businessName"],
                    address = f["address"],
                    complementNames = f["complementName"],
                    complementAddress = f["complementAddress"],
                    city = f["city"],
                    province = f["province"],
                    cap = f["cap"],
                    state = f["state"],
                    id = Convert.ToInt32(f["id"]),
                    sender = s,
                    senderAR = sAR,
                    fr = fronteRetro,
                    rr = ricevutaRitorno,
                    bn = tipoStampa,
                    tipoLettera = f["tipoLettera"]
                };

                var t = await Globals.HttpClientSend("POST", "/api/Backoffice/ModAndSend", m);
            }

            return View("Invii");
        }
    }
}