using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WasySender_2_0.Models;

namespace WasySender_2_0.Controllers
{
    public class SpecialOperationsController : Controller
    {
        // GET: SpecialOperations
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult SendOk()
        {
            return View();
        }

        public async Task<ActionResult> History()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var l = new List<SpecialOperations>();
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "/api/SpecialOperations?userId=" + u.id, u.areaTestUser);
            if (get.IsSuccessStatusCode)
                l = await get.Content.ReadAsAsync<List<SpecialOperations>>();


            return View(l);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> UploadFile()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var files = Request.Files[0];

            var zip = new ZipStampaUnione();

            if (files != null && files.ContentLength > 0)
            {
                var ex = Path.GetExtension(files.FileName).ToLower();
                if (ex == ".zip")
                {
                    var tick = DateTime.Now.Ticks;
                    var name = tick + ex;
                    var directory = Server.MapPath("/Upload/SpecialOperations/IN/" + u.id);

                    bool isExists = Directory.Exists(directory);
                    if (!isExists)
                        Directory.CreateDirectory(directory);

                    var specificDirectory = Server.MapPath("/Upload/SpecialOperations/IN/" + u.id + "/" + tick);

                    bool isExistsS = Directory.Exists(specificDirectory);
                    if (!isExistsS)
                        Directory.CreateDirectory(specificDirectory);

                    files.SaveAs(Path.Combine(specificDirectory + "/" + name));
                    Globals.ExtractFileZip(specificDirectory + "/" + name, specificDirectory);

                    int fileCount = Directory.GetFiles(specificDirectory, "*pdf").Length;

                    zip.filePath = specificDirectory;
                    zip.name = name;
                    zip.success = true;
                    zip.pathCsv = "Hai caricato " + fileCount + " pdf";
                    zip.pathDoc = "";
                }
            }
            else
                zip.errorMessage = "File .zip non trovato.";

            return new JavaScriptSerializer().Serialize(zip);
        }


        [HttpPost]
        public async Task<ActionResult> Send(FormCollection dataForm)
        {
            try
            {
                int sender = Convert.ToInt32(dataForm["sender"]);
                string opType = dataForm["operationType"];
                var SenderAR = dataForm["SenderAR"];
                var filePath = dataForm["filePath"];
                var fileName = dataForm["fileName"];
                var BiancoNero = dataForm["tipoStampa"];
                var FronteRetro = dataForm["fronteRetro"];
                var AR = dataForm["AR"];
                string fileDir = dataForm["filePath"];

                int op = 0;
                string text = "";
                string prodotto = "";
                switch (opType)
                {
                    case "P1":
                        op = (int)operationType.LOL;
                        text = "Posta1";
                        prodotto = "Lettera";
                        break;
                    case "P4":
                        op = (int)operationType.LOL;
                        text = "Posta4";
                        prodotto = "Lettera";
                        break;

                    case "R1":
                        op = (int)operationType.ROL;
                        text = "Raccomandata1";
                        prodotto = "Raccomandata";
                        break;
                    case "R4":
                        op = (int)operationType.ROL;
                        text = "Raccomandata4";
                        prodotto = "Raccomandata";
                        break;

                }

                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                HttpResponseMessage get = new HttpResponseMessage();

                //SENDER
                var s = new Sender();
                get = await Globals.HttpClientSend("GET", "api/SendersUsers/" + sender, u.areaTestUser);
                if (!get.IsSuccessStatusCode) {
                    Session["Error"] = "Errore nel recupero del mittente";
                    return View("Error");
                }

                s = await get.Content.ReadAsAsync<Sender>();

                //SENDER AR
                var sAR = new Sender();
                if (SenderAR != "")
                {
                    get = await Globals.HttpClientSend("GET", "api/SendersUsers/" + Convert.ToInt32(SenderAR), u.areaTestUser);
                    if (!get.IsSuccessStatusCode) {
                        Session["Error"] = "Errore nel recupero del destinatario della cartolina di ritorno";
                        return View("Error");
                    }

                    sAR = await get.Content.ReadAsAsync<Sender>();
                }

                //CREAZIONE FILE ZIP ED INVIO AD INDI
                var guid = Guid.NewGuid();
                try 
                { 
                    CreateCsv(s, prodotto, text, FronteRetro, BiancoNero, AR, fileDir + "/input.csv", sAR);

                    var l = new List<string>();
                    l.Add(fileDir + "\\input.csv");

                    var files = Directory.GetFiles(fileDir, "*pdf");
                    foreach (var f in files)
                        l.Add(f);

                    var zipFile =  Globals.CreateZipFile(Server.MapPath("/Upload/SpecialOperations/OUT/" + guid.ToString() + ".zip"), l);
                    Globals.SendViaFtp(Server.MapPath("/Upload/SpecialOperations/OUT/" + guid.ToString() + ".zip"), guid.ToString() + ".zip");

                }
                catch(Exception e)
                {
                    Session["Error"] = "Errore nella creazione del file zip.<br>" + e.Message.ToString();
                    return View("Error");
                }

                //CREAZIONE NUOVA OPERAZIONE
                var spo = new SpecialOperations()
                {
                    name = "Sp. del " + DateTime.Now.ToString("dd/MM/yyyy"),
                    userId = u.id,
                    operationType = op,
                    operationText = text,
                    zipFileName = Server.MapPath("/Upload/SpecialOperations/OUT/" + guid.ToString() + ".zip"),
                    guid  = guid
                };

                get = await Globals.HttpClientSend("POST", "api/SpecialOperations/New", u.areaTestUser, spo);
                if (!get.IsSuccessStatusCode)
                {
                    Session["Error"] = "Errore nella creazione dell'operazione.";
                    return View("Error");
                }


                return View("SendOk");
            }
            catch (Exception e)
            {
                Session["Error"] = "Errore Generico durante la richiesta";
                return View("Error");
            }
        }

        private void CreateCsv(Sender s, string prodotto, string tipo, string fronteRetro, string biancoNero, string AR, string filePath, Sender sAR = null)
        {
            StringBuilder sbRtn = new StringBuilder();
            // If you want headers for your file
            var header = string.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\";\"{5}\";\"{6}\";\"{7}\";\"{8}\";\"{9}\";\"{10}\";\"{11}\";\"{12}\";\"{13}\";\"{14}\";\"{15}\";\"{16}\";\"{17}\";\"{18}\";\"{19}\";\"{20}\"",
                                       "prodotto",
                                       "tipo",
                                       "fronteRetro",
                                       "biancoNero",
                                       "nominativo",
                                       "completamentoNominativo",
                                       "indirizzo",
                                       "completamentoIndirizzo",
                                       "cap",
                                       "comune",
                                       "provincia",
                                       "nazione",
                                       "AR",
                                        "nominativoAR",
                                       "completamentoNominativoAR",
                                       "indirizzoAR",
                                       "completamentoIndirizzoAR",
                                       "capAR",
                                       "comuneAR",
                                       "provinciaAR",
                                       "nazioneAR"
                                     );
            sbRtn.AppendLine(header);

            var nominativoAR = "";
            var completamentoNominativoAR = "";
            var indirizzoAR = "";
            var completamentoIndirizzoAR = "";
            var capAR = "";
            var comuneAR = "";
            var provinciaAR = "";
            var nazioneAR = "";

            if (sAR.id > 0)
            {
                nominativoAR = sAR.businessName + " " + sAR.name + " " + sAR.surname;
                completamentoNominativoAR = sAR.complementNames;
                indirizzoAR = sAR.dug + " " + sAR.address + " " + sAR.houseNumber;
                completamentoIndirizzoAR = sAR.complementAddress;
                capAR = sAR.cap;
                comuneAR = sAR.city;
                provinciaAR = sAR.province;
                nazioneAR = sAR.state;
            }

            var body = string.Format("\"{0}\";\"{1}\";\"{2}\";\"{3}\";\"{4}\";\"{5}\";\"{6}\";\"{7}\";\"{8}\";\"{9}\";\"{10}\";\"{11}\";\"{12}\";\"{13}\";\"{14}\";\"{15}\";\"{16}\";\"{17}\";\"{18}\";\"{19}\";\"{20}\"",
                                       "" + prodotto + "",
                                       "" + tipo + "",
                                       "" + fronteRetro + "",
                                       "" + biancoNero + "",
                                       "" + s.businessName + " " + s.name + " " + s.surname + "",
                                       "" + s.complementNames + "",
                                       "" + s.dug + " " + s.address + " " + s.houseNumber + "",
                                       "" + s.complementAddress + "",
                                       "" + s.cap + "",
                                       "" + s.city + "",
                                       "" + s.province + "",
                                       "" + s.state + "",
                                       "" + AR + "",
                                       "" + nominativoAR + "",
                                       "" + completamentoNominativoAR + "",
                                       "" + indirizzoAR + "",
                                       "" + completamentoIndirizzoAR + "",
                                       "" + capAR + "",
                                       "" + comuneAR + "",
                                       "" + provinciaAR + "",
                                       "" + nazioneAR + ""
                                     );
            sbRtn.AppendLine(body);

            System.IO.File.WriteAllText(filePath, sbRtn.ToString());

        }
    }
}