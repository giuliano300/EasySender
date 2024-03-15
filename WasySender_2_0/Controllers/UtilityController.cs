using BitMiracle.Docotic.Pdf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WasySender_2_0.DataModel;
using WasySender_2_0.Models;

namespace WasySender_2_0.Controllers
{
    public class UtilityController : Controller
    {
        // GET: Utility
        public ActionResult Index()
        {
            return View();
        }
        // GET: Utility
        public ActionResult StampaUnione()
        {
            return View();
        }

        public ActionResult Ecr()
        {
            return View();
        }

        public ActionResult OptimizePdf()
        {
            return View();
        }

        public ActionResult Files()
        {
            return View();
        }

        public ActionResult SincronizzaBollettiniPagamenti()
        {
            return View();
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
                    var directory = Server.MapPath("/Upload/Utility/" + u.id);

                    bool isExists = Directory.Exists(directory);
                    if (!isExists)
                        Directory.CreateDirectory(directory);

                    var specificDirectory = Server.MapPath("/Upload/Utility/" + u.id + "/" + tick);

                    bool isExistsS = Directory.Exists(specificDirectory);
                    if (!isExistsS)
                        Directory.CreateDirectory(specificDirectory);

                    files.SaveAs(Path.Combine(specificDirectory + "/" + name));
                    Globals.ExtractFileZip(specificDirectory + "/" + name, specificDirectory);

                    //CONTROLLO SE SONO STATI CARICATI SIA IL CSV CHE IL DOC
                    string[] csvFile = Directory.GetFiles(specificDirectory, "*.csv");
                    if (csvFile.Length == 0)
                    {
                        zip.errorMessage = "File .csv non trovato.";
                        return new JavaScriptSerializer().Serialize(zip);
                    };

                    string[] docFile = Directory.GetFiles(specificDirectory, "*.doc");
                    string[] docxFile = Directory.GetFiles(specificDirectory, "*.docx");
                    if (docFile.Length == 0 && docxFile.Length == 0)
                    {
                        zip.errorMessage = "File .doc o docx non trovato.";
                        return new JavaScriptSerializer().Serialize(zip);
                    };
                    //FINE CONTROLLO

                    zip.filePath = specificDirectory;
                    zip.name = name;
                    zip.success = true;
                    zip.pathCsv = csvFile[0];
                    zip.pathDoc = docFile.Length > 0 ? docFile[0] : docxFile[0];
                }
            }
            else
                zip.errorMessage = "File .zip non trovato.";

            return new JavaScriptSerializer().Serialize(zip);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> GeneraStampaUnione()
        {
            var z = new ZipCompressione();
            try { 

                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                var pathDoc = Request.Form["pathDoc"];
                var pathCsv = Request.Form["pathCsv"];
                var filePath = Request.Form["filePath"];

                //LISTA DI FILE DA COMPRIMERE
                // IN FORMATO ZIP
                var l =  Globals.ReadCsvCreatePDF(pathCsv, pathDoc, filePath);

                //CREAZIONE ZIP
                var pathToSave = filePath + "/Pdf/";
                var end = Globals.CreateZipFile(pathToSave, l);

                z.success = true;
                z.pathCompressedFile = "/" + end.Substring(end.IndexOf("Upload"));

            }
            catch(Exception e)
            {
                z.erroMessage = e.Message;
            }
            return new JavaScriptSerializer().Serialize(z);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> GetText()
        {
            var z = new ZipCompressione();
            try
            {

                Users u = new Users();
                u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

                int a = Convert.ToInt32(Request.Form["a"]);
                int b = Convert.ToInt32(Request.Form["b"]);
                int c = Convert.ToInt32(Request.Form["c"]);
                int d = Convert.ToInt32(Request.Form["d"]);

                //LISTA DI FILE DA COMPRIMERE
                // IN FORMATO ZIP
                var l = Globals.GetTextFromRectangle(Server.MapPath("/Upload/test.jpg"), a, b, c, d);

                z.success = true;
                z.pathCompressedFile = l;

            }
            catch (Exception e)
            {
                z.erroMessage = e.Message;
            }
            return new JavaScriptSerializer().Serialize(z);
        }

        [ValidateInput(false)]
        public async Task<string> UploadPdf()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var res = new PdfOptimizationResponse();
            try 
            { 
                var files = Request.Files[0];

                if (files != null && files.ContentLength > 0)
                {
                    var ex = Path.GetExtension(files.FileName).ToLower();
                    if (ex == ".pdf")
                    {
                        var tick = DateTime.Now.Ticks;
                        var name = tick + ex;
                        var directory = Server.MapPath("/Upload/Utility/" + u.id);

                        bool isExists = Directory.Exists(directory);
                        if (!isExists)
                            Directory.CreateDirectory(directory);

                        var path = "/Upload/Utility/" + u.id + "/" + tick;

                        var specificDirectory = Server.MapPath(path);

                        bool isExistsS = Directory.Exists(specificDirectory);
                        if (!isExistsS)
                            Directory.CreateDirectory(specificDirectory);

                        files.SaveAs(Path.Combine(specificDirectory + "/" + name));

                        using (var pdf = new PdfDocument(Path.Combine(specificDirectory + "/" + name)))
                        {
                            pdf.SaveOptions.UseObjectStreams = true;
                            pdf.Save(Path.Combine(specificDirectory + "/" + tick + "-compressed.pdf"));
                        }

                        res.pathFile = path + "/" + tick + "-compressed.pdf";

                    }
                }
                else
                    res.errorMessage = "File .pdf non trovato.";

            }
            catch(Exception e)
            {
                res.errorMessage = e.Message.ToString();
                res.success = false;
            }
            return new JavaScriptSerializer().Serialize(res);

        }

        [ValidateInput(false)]
        public async Task<string> UploadTxtBipiol()
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var res = new TxtBipiolResponse();

            var files = Request.Files[0];

            if (files != null && files.ContentLength > 0)
            {
                var ex = Path.GetExtension(files.FileName).ToLower();
                if (ex == ".txt")
                {
                    var tick = DateTime.Now.Ticks;
                    var name = tick + ex;
                    var directory = Server.MapPath("/Upload/Utility/" + u.id);

                    bool isExists = Directory.Exists(directory);
                    if (!isExists)
                        Directory.CreateDirectory(directory);

                    var path = "/Upload/Utility/" + u.id + "/" + tick;

                    var specificDirectory = Server.MapPath(path);

                    bool isExistsS = Directory.Exists(specificDirectory);
                    if (!isExistsS)
                        Directory.CreateDirectory(specificDirectory);

                    files.SaveAs(Path.Combine(specificDirectory + "/" + name));

                    res.success = true;
                    res.pathTxt = specificDirectory + "/" + name;
                    res.name = name;
                    res.errorMessage = "Caricamento effettuato con successo.";

                }
            }
            else
                res.errorMessage = "File .txt non trovato.";

            return new JavaScriptSerializer().Serialize(res);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<string> MatchFile(string pathTxt)
        {
            Users u = new Users();
            u = JsonConvert.DeserializeObject<Users>(Request.Cookies["login"].Value);

            var res = new TxtBipiolResponse();

            string[] lines = System.IO.File.ReadAllLines(pathTxt);

            // Display the file contents by using a foreach loop.
            List<Codice> codici = new List<Codice>();
            foreach (string line in lines)
            {
                var l = line.Split(';');
                if (l.Length > 10)
                    if(l[5]!="" && l[5] != null)
                    {
                        var arrData = l[1].Split('/');
                        var d = arrData[2] + "-" + arrData[1] + "-" + arrData[0];

                        try
                        {
                            var dd = Convert.ToDateTime(d);
                            var codice = new Codice()
                            {
                                dataPagamento = dd,
                                dataValidita = l[2],
                                tipo = l[3],
                                codice = l[4],
                                codiceCliente = l[5],
                                valuta = l[6],
                                prezzo = l[7]
                            };
                            codici.Add(codice);
                        }
                        catch (Exception e)
                        {
                            var t = e.Message.ToString();
                        }
                    }
            }

            if (codici.Count() == 0)
            {
                res.success = false;
                res.errorMessage = "Nessun destinatario disponibile";

                return new JavaScriptSerializer().Serialize(res);
            }

            var j = JsonConvert.SerializeObject(codici);

            var lc = new List<CompleteNameBulletin>();
            HttpResponseMessage get = new HttpResponseMessage();
            get = await Globals.HttpClientSend("POST", "Payments/Bulletin?guidUser=" + u.guidUser, true, codici.ToList());
            if (get.IsSuccessStatusCode)
                lc = await get.Content.ReadAsAsync<List<CompleteNameBulletin>>();

            var csv = new StringBuilder();

            string h = "Data Pagamento;Codice Cliente;Nominativo;Indirizzo;CAP;Citta;Provincia;Importo;Numero Conto Corrente;Intestatario;Causale\n";
            foreach (var e in lc)
            {
                h +=  Convert.ToDateTime(e.bulletin.DataPagamento).ToString("dd/MM/yyyy") + ";CC" + e.bulletin.CodiceCliente.ToString() + ";" + e.name.businessName + " " + e.name.name + " " + e.name.surname + ";" + e.name.address + ";" + e.name.cap + ";" + e.name.city + ";" + e.name.province + ";" +  e.bulletin.ImportoEuro + ";" + e.bulletin.NumeroContoCorrente + ";" + e.bulletin.IntestatoA + ";" + e.bulletin.Causale + "\n";
            }

            var finalPath = "/Upload/Utility/" + u.id + "/" + DateTime.Now.Ticks + ".csv";
            var directory = Server.MapPath(finalPath);
            csv.AppendLine(h);
            System.IO.File.WriteAllText(directory, csv.ToString());

            res.success = true;
            res.bulletinsFile = finalPath;

            return new JavaScriptSerializer().Serialize(res);
        }

    }
}