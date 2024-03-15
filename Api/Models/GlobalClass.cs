using Api.Dtos;
using AutoMapper;
using FluentFTP;
using GemBox.Pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Api.Models
{
    public class GlobalClass
    {

        public static string siteName = "EWT";

        //public static string baseUrl = "https://apinew.easysender.it/";
        public static string baseUrl = "http://localhost:5001/";
        public static string DescrizioneTipoPagamento = "Post fatturato al cliente";
        public static string IdTipoPagamento = "6";
        public static int countBlocksMolCol = 20;
        public static int vat = 22;

        //FTP
        public const string ftpUrl = "ftp://ftp.indi.it/";
        public const string ftpOutputFolder = "/Output/";
        public const string ftpInputFolder = "/Input/";
        public const string usernameFtp = "EWT";
        public const string passwordFtp = "Ewt_2020";
        public const string OutputFolder = "/temp/Output/";
        public const string InputFolder = "/temp/Input/";

        public const string ftpRRInputFolder = "/output_scansioni/";
        public const string localRRPath = "/Public/scansioni";

        //FTPS
        public const string TargetFtpSource = "rmftp1.postel.com";


        //FTP DOCS
        public const string ftpUrlDoc = "ftp://ftp.private.easysender.it/public_html/private-files/";
        public const string usernameFtpDoc = "private001";
        public const string passwordFtpDoc = "Zt~G[_n,7e(r";


        //SMS
        public static string clid = "tekmerion";
        public static string pwhc = "marc4tekbrig";
        public static string apiSmsUrl = "https://mmk.mail-maker.com/sms4b-rest/";

        //EMAIL
        public static string serialCode = "EN-2018Dec12-5rhxfP82QSy+1DXrPWxtA639rl+nWT4KrZVxH07tjZnTQAWILabzeJimpEr8sh59OADfBgZ1ZarFgnU14SnaPJp/zWw==A";


        //AGENZIA ENTRATE
        public static string AUTH_ID_AGENZIA_ENTRATE = "UHBYSJTH";

        //SMS
        public static decimal smsPrice = 0.07M;
        public static string portalDownloadFile = "";

        //MULTIGESTIONE
        public static string usernameMultigestione = "demoapi2@texsrv3";
        public static string pwdMultigestione = "da2.2020";
        public static string sourceMultigestione = "APP";
        public static string idDestinatarioMultigestione = "";
        public static int isPrivatoMultigestione = 1;
        public static string modalitaPagamentoMultigestione = "MP01";

        //CASSETTO FISCALE
        public static byte[] AdE_CodiceFiscale = null;
        public static byte[] AdE_PIN = null;
        public static byte[] AdE_Password = null;


        public static DateTime UnixTimeToDateTime(double unixTimeStamp)
        {
            var u = Convert.ToInt64(unixTimeStamp);
            var p = UnixDateTime.FromUnixTimeMilliseconds(u).Date;

            return p;
        }


        public static DateTime GetLastDayOfMonth(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
        }

        public static async Task<HttpResponseMessage> HttpClientSendGenerical(string type, string url, object o = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage r = new HttpResponseMessage();
            switch (type.ToUpper())
            {
                case "POST":
                    r = await client.PostAsJsonAsync(url, o);
                    break;
                case "PUT":
                    r = await client.PutAsJsonAsync(url, o);
                    break;
                case "GET":
                    r = await client.GetAsync(url);
                    break;
                default:
                    break;
            }

            return r;

        }

        public static string CreateZipFile(List<string> files, string path)
        {
            try
            {
                var zipFile = HostingEnvironment.MapPath(path + ".zip");
                using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                {
                    foreach (var fPath in files)
                    {
                        archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
                    }
                }
                return zipFile;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public static bool ExtractZipFile(string pathFile, string extractPath)
        {
            try
            {
                ZipFile.ExtractToDirectory(pathFile, extractPath);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static string GetCSV(string url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }


        public static void SendViaFtp(string localFilePath, string uploadFileName, string ftpUsername = usernameFtp, string ftpPassword = passwordFtp, string pathFtp = ftpUrl + ftpInputFolder)
        {
            Uri uri = new Uri(pathFtp + uploadFileName);
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            client.UploadFile(uri, localFilePath);
        }

        public static string DownloadFileViaFtp(string localFilePath, string fileName, string remotePath, string ftpUsername = usernameFtp, string ftpPassword = passwordFtp)
        {
            try
            {
                WebClient client = new WebClient();
                client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                client.DownloadFile(new Uri(remotePath), localFilePath + "/" + fileName);
                return fileName;
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        public static bool GetAllDirectoriesAndFiles(string localFilePath, string remotePath, string remoteFolder, string ftpUsername = usernameFtp, string ftpPassword = passwordFtp)
        {
            try 
            {
                string[] files = GetFileList(remotePath, remoteFolder, ftpUsername, ftpPassword);
                foreach (string file in files.Where(a=>a.Contains(".pdf")))
                {
                    DownloadFileViaFtp(localFilePath, file , remotePath + remoteFolder + "/" + file, ftpUsername, ftpPassword);


                    FtpWebRequest reqFTPM = (FtpWebRequest)FtpWebRequest.Create(new Uri(remotePath + remoteFolder + file));
                    reqFTPM.Method = WebRequestMethods.Ftp.Rename;
                    reqFTPM.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    reqFTPM.RenameTo = "/output_scansioni_lavorati/" + file;
                    reqFTPM.GetResponse().Close();

                }
                return true;
            }
            catch(Exception e)
            {

            }
            return false;
        }


        public static string[] GetFileList(string remotePath, string remoteFolder, string ftpUsername = usernameFtp, string ftpPassword = passwordFtp)
        {
            string[] downloadFiles;
            StringBuilder result = new StringBuilder();
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(remotePath + remoteFolder));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                reqFTP.Proxy = null;
                reqFTP.KeepAlive = false;
                reqFTP.UsePassive = false;
                response = reqFTP.GetResponse();
                reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line.Replace(remoteFolder + "/", ""));
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                // to remove the trailing '\n'
                result.Remove(result.ToString().LastIndexOf('\n'), 1);

                var res = result.ToString().Split('\n');

                return res;
            }
            catch (Exception ex)
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                downloadFiles = null;
                return downloadFiles;
            }
        }

        public static void SetNamesState(string path, int operationId)
        {
            var _context = new Entities();
            string fileList = GetCSV(path);
            string[] tempStr;

            tempStr = fileList.Split((char)10);
            int i = 0;
            int positionId = 0;
            int positionDataConsegna = 0;
            int positionCodice = 0;

            var o = _context.Operations.SingleOrDefault(a => a.id == operationId);

            foreach (string row in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(row))
                {
                    try
                    {
                        string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                        if (i == 0)
                        {
                            for (var x = 0; x < column.Length; x++)
                            {
                                switch (column[x].ToUpper())
                                {
                                    case "KEY":
                                        positionId = x;
                                        break;
                                    case "DATA_SPED":
                                        positionDataConsegna = x;
                                        break;
                                    case "TRACKCODE":
                                        positionCodice = x;
                                        break;
                                }
                            };
                        }
                        else
                        {
                            int id = Convert.ToInt32(column[positionId]);
                            var n = _context.Names.SingleOrDefault(a => a.id == id);
                            n.codice = column[positionCodice];
                            n.currentState = 0;
                            n.requestId = "";

                            if (column[positionDataConsegna] != "")
                            {
                                n.consegnatoDate = Convert.ToDateTime(column[positionDataConsegna]);
                                n.currentState = 1;
                                n.stato = "Presa in carico Poste";
                                if (o.operationType == (int)operationType.LOL)
                                    n.finalState = true;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        o.error = true;
                        o.errorMessage = e.Message;
                    }

                    _context.SaveChanges();
                }
                i++;
            }
        }

        public static void SetAcosetState(string path)
        {
            var _context = new Entities();
            string fileList = GetCSV(path);
            string[] tempStr;

            tempStr = fileList.Split((char)10);
            int i = 0;
            int positionDescrizione = 0;
            int positionCodice = 0;

            foreach (string row in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(row))
                {
                    try
                    {
                        string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                        if (i == 0)
                        {
                            for (var x = 0; x < column.Length; x++)
                            {
                                switch (column[x].ToUpper())
                                {
                                    case "DESCRIZIONE":
                                        positionDescrizione = x;
                                        break;
                                    case "CODICE":
                                        positionCodice = x;
                                        break;
                                }
                            };
                        }
                        else
                        {
                            string code = column[positionCodice];
                            var n = _context.Names.SingleOrDefault(a => a.codice == code);
                            if (n != null) { 
                                n.finalState = true;
                                n.stato = column[positionDescrizione];
                                n.currentState = 2;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    _context.SaveChanges();
                }
                i++;
            }
        }
        public static string GetFileName(string name)
        {
            var a = name.Split('/');
            return a[a.Length - 1];
        }

        public static List<string> GetListOperationPath(string uri)
        {
            var a = uri.Split('/');
            var len = a.Length;
            var list = new List<string>();
            var path = "";

            if (uri.Contains("Users"))
            {
                for (var x = 0; x < a.Length - 1; x++)
                    path += a[x] + "/";

                foreach (var file in Directory.GetFiles(path, "*.pdf", SearchOption.AllDirectories))
                    list.Add(file);
            }
            else
                list.Add(uri);

            return list;
        }

        public static void CreateTxtFile(string contenuto, string nomeFile, string folder)
        {
            if (nomeFile == "")
                nomeFile = DateTime.Now.Ticks.ToString();
            string path = HostingEnvironment.MapPath("/public/logs/" + folder + "/" + nomeFile + ".txt");
            TextWriter tw = new StreamWriter(path, true);
            tw.WriteLine(contenuto);
            tw.Close();
        }

        public static string CreateCsvFileFromOperation(int id)
        {
            var type = "";
            var _context = new Entities();
            var names = _context.Names.Where(a => a.operationId == id);
            if (names.Count() == 0)
                return null;

            switch (names.ToList()[0].Operations.operationType)
            {
                case 1:
                    type = "RS";
                    if ((bool)names.ToList()[0].ricevutaRitorno)
                        type = "RR";
                    break;
                case 2:
                    type = "P4";
                    if (names.ToList()[0].tipoLettera == "Posta1")
                        type = "P1";
                    break;
            };

            var fileName = type + "-" + DateTime.Now.Ticks + ".csv";

            StringBuilder csvContent = new StringBuilder();
            string csvPath = "";
            try
            {
                string item = "COGNOMENOME;INDIRIZZO;FRAZIONE;CAP;LOCALITA;PROV;TOT PAGINE;NOME_PDF;DATA_SPED;KEY;PRESSO;TARIFFA;DATA_ACCETTAZIONE;DATA_CONSEGNA;ESITO_CONSEGNA;CODICE_A_BARRE;CAUSALE;ANOMALIE;TRACKCODE;PATH_PDF;mitt_TITOLO;mitt_NOME;mitt_C/O;mitt_INDIRIZZO;mitt_FRAZIIONE;mitt_CAP;mitt_LOCALITA;mitt_PROVINCIA;mitt_STATO;COLORE;FR;RR;";
                csvContent.AppendLine(item);
                foreach (var n in names)
                {
                    //NOME DEL FILE
                    var af = n.fileName.Split('/');

                    //RICEVUTA DI RITORNO
                    var rr = "NO";
                    if ((bool)n.ricevutaRitorno)
                        rr = "SI";

                    //FRONTE RETRO
                    var fr = "SI";
                    if ((bool)n.fronteRetro)
                        fr = "NO";

                    //BIANCO NERO
                    var bn = "NO";
                    if (!(bool)n.tipoStampa)
                        bn = "SI";

                    //SENDER
                    var nominativoMittente = "";
                    var indirizzoMittente = "";
                    var capMittente = "";
                    var cittaMittente = "";
                    var provinciaMittente = "";
                    var statoMittente = "";

                    var s = _context.Senders.Where(a => a.operationId == id).FirstOrDefault(a => a.AR != true);
                    if (s == null)
                        return csvPath;

                    nominativoMittente = s.businessName + " " + s.name + " " + s.surname;
                    indirizzoMittente = s.dug + " " + s.address + " " + s.houseNumber;
                    capMittente = s.cap;
                    cittaMittente = s.city;
                    provinciaMittente = s.province;
                    statoMittente = s.state;

                    if ((bool)n.ricevutaRitorno)
                    {
                        var sAR = _context.Senders.Where(a => a.operationId == id).FirstOrDefault(a => a.AR == true);
                        if (sAR != null)
                            s = sAR;

                        nominativoMittente = s.businessName + " " + s.name + " " + s.surname;
                        indirizzoMittente = s.dug + " " + s.address + " " + s.houseNumber;
                        capMittente = s.cap;
                        cittaMittente = s.city;
                        provinciaMittente = s.province;
                        statoMittente = s.state;
                    }

                    item = n.businessName + " " + n.name + " " + n.surname + ";" + n.dug + " " + n.address + " " +
                       n.houseNumber + ";;" + n.cap + ";" + n.city + ";" + n.province + ";" + n.numberOfPages + ";" + af[af.Length - 1] + ";;" + n.id + ";;" + type + ";;;;;;;;;;" + nominativoMittente + ";;" + indirizzoMittente + ";;" + capMittente + ";" +
                        cittaMittente + ";" + provinciaMittente + ";" + statoMittente + ";" + bn + ";" + fr + ";" + rr + ";";
                    item = item.Substring(0, item.Length - 1);
                    csvContent.AppendLine(item);
                };

                csvPath = HttpContext.Current.Server.MapPath(OutputFolder + fileName);
                File.Create(csvPath).Dispose();
                File.AppendAllText(csvPath, csvContent.ToString());
            }
            catch (Exception e)
            {
                var r = e;
            }
            return csvPath;
        }

        public static async Task<HttpResponseMessage> HttpClientSend(string type, string url, object o = null)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiSmsUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(clid + ":" + pwhc));
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + svcCredentials);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage r = new HttpResponseMessage();
            switch (type.ToUpper())
            {
                case "POST":
                    r = await client.PostAsJsonAsync(apiSmsUrl + url, o);
                    break;
                case "PUT":
                    r = await client.PutAsJsonAsync(apiSmsUrl + url, o);
                    break;
                case "GET":
                    r = await client.GetAsync(apiSmsUrl + url + o);
                    break;
                default:
                    break;
            }

            return r;

        }
        public static int pageNumber(int page = 0)
        {
            return (page > 1 ? page : 1);
        }

        public static int pageSize = 3;

        public static string rewriteUrl(string Stringa)
        {
            Stringa = Stringa.Replace("'", "-");
            Stringa = Stringa.Replace(".", "-");
            Stringa = Stringa.Replace(",", "-");
            Stringa = Stringa.Replace("è", "e");
            Stringa = Stringa.Replace("à", "a");
            Stringa = Stringa.Replace("ò", "o");
            Stringa = Stringa.Replace(" ", "-");
            Stringa = Stringa.Replace("/", "-");
            Stringa = Stringa.Replace("&", "-");
            Stringa = Stringa.Replace(":", "-");
            Stringa = Stringa.Replace(";", "-");
            return Stringa;
        }

        public static string leftString(string Stringa, int index)
        {
            return Stringa.Substring(0, Math.Min(index, Stringa.Length));
        }

        public static string textToEncrypt = "h2hposteitaliane";

        public static bool onlyNumbers(string text)
        {
            if (!Regex.IsMatch(text, "^[0-9]+$"))
                return false;
            return true;
        }

        public static bool onlyLetters(string text)
        {
            if (!Regex.IsMatch(text, @"^[a-zA-Z]+$"))
                return false;
            return true;
        }

        public static byte[] generatePwd()
        {
            return Encoding.ASCII.GetBytes(GeneraCodiceRandom());
        }

        private static Random random = new Random();
        public static string GeneraCodiceRandom()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghilmnopqrstuvwyz";
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GeneraCodiceRandomN(int number)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghilmnopqrstuvwyz";
            return new string(Enumerable.Repeat(chars, number)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static crt verificaCap(string Cap, int i)
        {
            crt ctrl = new crt();
            if (Cap.Length == 0)
            {
                ctrl.Errore = "Cap inesistente";
                ctrl.Valido = false;
            }
            if (Cap.Length != 5)
            {
                ctrl.Errore = "Lughezza cap non valida";
                ctrl.Valido = false;
            }

            if (i == 0)
            {
                ctrl.Errore = "Cap non valido";
                ctrl.Valido = false;
            }
            return ctrl;
        }

        public static crt verificaRagioneSociale(string ragioneSociale = "", string nome = "", string cognome = "")
        {
            crt ctrl = new crt();
            if (ragioneSociale.Length > 44)
            {
                ctrl.Errore = "Ragione sociale più di 44 caratteri";
                ctrl.Valido = false;
            }
            if (ragioneSociale.Length == 0 & (nome.Length == 0 | cognome.Length == 0))
            {
                ctrl.Errore = "Inserire Nome e Cognome o Ragione Sociale";
                ctrl.Valido = false;
            }
            return ctrl;
        }

        public static crt verificaIndirizzo(string dug, string indirizzo, string numeroCivico = "")
        {
            crt ctrl = new crt();
            if (indirizzo.Length == 0)
            {
                ctrl.Errore = "Indirizzo vuoto";
                ctrl.Valido = false;
            }
            else
            {
                //// CONTROLLO NUMERO CIVICO
                //if(numeroCivico != null)
                //    if (numeroCivico.Length > 5)
                //    {
                //        ctrl.Errore = "il numero civico supera i 5 caratteri";
                //        ctrl.Valido = false;
                //    }

                //if (dug != null) { 
                //    if (dug.Length <= 0)
                //    {
                //        ctrl.Errore = "il DUG non è valido";
                //        ctrl.Valido = false;
                //    }
                //    if (dug.Length > 10)
                //    {
                //        ctrl.Errore = "il DUG supera i 10 caratteri";
                //        ctrl.Valido = false;
                //    }
                //}
            }
            return ctrl;
        }

        public static crt verificaCitta(string city)
        {
            crt ctrl = new crt();
            if (city.Length == 0)
            {
                ctrl.Errore = "città vuota";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaProvincia(string provincia)
        {
            crt ctrl = new crt();
            if (provincia.Length == 0)
            {
                ctrl.Errore = "provincia vuota";
                ctrl.Valido = false;
                return ctrl;
            }
            if (provincia.Length > 2)
            {
                ctrl.Errore = "il campo provincia deve avere 2 caratteri";
                ctrl.Valido = false;
                return ctrl;
            }
            if (!onlyLetters(provincia))
            {
                ctrl.Errore = "nel campo provincia sono inseriti caratteri non validi";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaStato(string stato)
        {
            crt ctrl = new crt();
            if (stato.Length == 0)
            {
                ctrl.Errore = "stato vuoto";
                ctrl.Valido = false;
                return ctrl;
            }
            if (!onlyLetters(stato))
            {
                ctrl.Errore = "nel campo stato sono inseriti caratteri non validi";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaCodiceCliente(string codiceCliente)
        {
            crt ctrl = new crt();
            // CodiceCliente
            if (codiceCliente == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo CodiceCliente vuoto - Bollettino";
            }

            // CodiceCliente
            if (!onlyNumbers(codiceCliente))
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo CodiceCliente deve contenere solo numeri";
            }

            // CodiceCliente
            if (codiceCliente.Length != 18)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo CodiceCliente non ha la lunghezza consentita di 18 numeri";
            }

            long firstCod = Convert.ToInt64(codiceCliente.Substring(0, 16));
            int lastCod = Convert.ToInt32(codiceCliente.Substring(codiceCliente.Length - 2, 2));

            long codiceControllo = firstCod % 93;
            if (codiceControllo != lastCod)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Nel Campo CodiceCliente il controcodice non è valido (Primi 16 caratteri mod 93)";
            }

            return ctrl;
        }

        public static crt verificaFile(byte[] nomeFile, string pathUrl)
        {
            crt ctrl = new crt();
            //if (nomeFile.Length == 0)
            //{
            //    ctrl.Errore = "file non inserito";
            //    ctrl.Valido = false;
            //    return ctrl;
            //}

            //var ex = nomeFile.Split('.');
            //if (ex.Length < 2)
            //{
            //    ctrl.Errore = "file non valido";
            //    ctrl.Valido = false;
            //    return ctrl;
            //}
            //if (ex[1].ToLower() !="pdf")
            //{
            //    ctrl.Errore = "estensione del file non valida";
            //    ctrl.Valido = false;
            //    return ctrl;
            //}

            //if(!File.Exists(HttpContext.Current.Server.MapPath(pathUrl + nomeFile)))
            //{
            //    ctrl.Errore = "nessun file trovato con questo nome";
            //    ctrl.Valido = false;
            //    return ctrl;
            //}

            return ctrl;
        }

        public static crt verificaNazioneInteraLista(List<NamesDto> Destinatari)
        {
            crt ctrl = new crt();
            int countIt = Destinatari.Where(a => a.state.ToUpper() == "ITALIA").Count();
            int countEstero = Destinatari.Where(a => a.state.ToUpper() != "ITALIA").Count();
            if (countIt > 0 & countEstero > 0)
                ctrl.ItaliaEstero = true;
            return ctrl;
        }

        public static ListaControlloDestinatari verificaDestinatari(List<NamesDto> Destinatari)
        {
            var file = HttpRuntime.AppDomainAppPath + "/json/comuniItaliani.json";
            var r = File.ReadAllText(file);
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();
            ListaControlloDestinatari l = new ListaControlloDestinatari();

            crt cn = verificaNazioneInteraLista(Destinatari);
            if (!cn.Valido)
            {
                l.ListCrtlD = null;
                l.ItaliaEstero = true;
                l.ErroreItaliaEstero = "Nella lista dei destinatari ci sono sia nominativi italiani che esteri.";
                return l;
            }

            for (int x = 0; x <= Destinatari.Count - 1; x++)
            {
                NamesDto Destinatario = Destinatari[x];
                ControlloDestinatario ctrl = new ControlloDestinatario();
                ctrl.Destinatario = Destinatario;

                string Cap = Destinatario.cap.Replace(" ", "");
                var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
                int i = comune.Count();

                // CONTROLLO CAP
                crt crt = verificaCap(Cap, i);
                if (!crt.Valido)
                {
                    ctrl.Valido = crt.Valido;
                    ctrl.Errore = crt.Errore;
                }

                // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
                crt crtR = verificaRagioneSociale(Destinatario.businessName, Destinatario.name, Destinatario.surname);
                if (!crtR.Valido)
                {
                    ctrl.Valido = crtR.Valido;
                    ctrl.Errore = crtR.Errore;
                }

                // CONTROLLO INDIRIZZO
                crt crtA = verificaIndirizzo(Destinatario.dug, Destinatario.address, Destinatario.houseNumber);
                if (!crtA.Valido)
                {
                    ctrl.Valido = crtA.Valido;
                    ctrl.Errore = crtA.Errore;
                }

                listctrl.Add(ctrl);
            }
            l.ListCrtlD = listctrl;

            return l;
        }

        public static ControlloDestinatario verificaDestinatario(NamesDto recipient, string pathUrl = null)
        {
            var file = HttpRuntime.AppDomainAppPath + "/json/comuniItaliani.json";
            var r = File.ReadAllText(file);
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();
            ListaControlloDestinatari l = new ListaControlloDestinatari();

            NamesDto Destinatario = recipient;
            ControlloDestinatario ctrl = new ControlloDestinatario();
            ctrl.Destinatario = Destinatario;

            string Cap = Destinatario.cap.Replace(" ", "");
            var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
            int i = comune.Count();

            // CONTROLLO CAP
            crt crt = verificaCap(Cap, i);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Destinatario";
            }

            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = verificaRagioneSociale(Destinatario.businessName, Destinatario.name, Destinatario.surname);
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Destinatario";
            }

            // CONTROLLO INDIRIZZO
            crt crtA = verificaIndirizzo(Destinatario.dug, Destinatario.address, Destinatario.houseNumber);
            if (!crtA.Valido)
            {
                ctrl.Valido = crtA.Valido;
                ctrl.Errore = crtA.Errore + " - Destinatario";
            }
            // CONTROLLO CITTA'
            crt crtC = verificaCitta(Destinatario.city);
            if (!crtC.Valido)
            {
                ctrl.Valido = crtC.Valido;
                ctrl.Errore = crtC.Errore + " - Destinatario";
            }

            // CONTROLLO PROVINCIA
            crt crtP = verificaProvincia(Destinatario.province);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Destinatario";
            }

            // CONTROLLO STATO
            crt crtS = verificaStato(Destinatario.state);
            if (!crtS.Valido)
            {
                ctrl.Valido = crtS.Valido;
                ctrl.Errore = crtS.Errore + " - Destinatario";
            }

            // CONTROLLO FILE
            crt crtF = verificaFile(Destinatario.attachedFile, pathUrl);
            if (!crtF.Valido)
            {
                ctrl.Valido = crtF.Valido;
                ctrl.Errore = crtF.Errore + " - Destinatario";
            }

            return ctrl;
        }

        public static ControlloBollettino verificaBollettino(BulletinsDtos b)
        {
            ControlloBollettino ctrl = new ControlloBollettino();
            ctrl.Bollettino = b;

            // CONTROLLO CONTO CORRENTE
            if (b.numeroContoCorrente == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente non valido - Bollettino";
            }

            if (!onlyNumbers(b.numeroContoCorrente))
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente deve contenere solo numeri";
            }

            if (b.numeroContoCorrente.Length != 12)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente non ha la lunghezza consentita di 12 numeri";
            }

            // Intestato A
            if (b.intestatoA == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo IntestatoA vuoto - Bollettino";
            }

            // CodiceCliente
            //Controllo il codice cliente solo se non è PagoPA
            if (b.PagoPA != true)
            {
                crt crtCC = GlobalClass.verificaCodiceCliente(b.codiceCliente);
                if (!crtCC.Valido)
                {
                    ctrl.Valido = false;
                    ctrl.Errore = crtCC.Errore;
                }
            }

            // ImportoEuro
            if (b.importoEuro == 0)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo ImportoEuro vuoto - Bollettino";
            }

            // EseguitoDaNominativo
            if (b.eseguitoDaNominativo == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaNominativo vuoto - Bollettino";
            }

            // EseguitoDaIndirizzo
            if (b.eseguitoDaIndirizzo == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaIndirizzo vuoto - Bollettino";
            }

            // EseguitoDaIndirizzo
            crt crt = verificaCap(b.eseguitoDaCAP, 1);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Bollettino";
            }

            // EseguitoDaLocalita
            if (b.eseguitoDaLocalita == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaLocalita vuoto - Bollettino";
            }

            // Causale
            if (b.causale == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo Causale vuoto - Bollettino";
            }

            // BulletinType
            if (b.bulletinType != (int)bulletinType.Bollettino451 && b.bulletinType != (int)bulletinType.Bollettino674 && b.bulletinType != (int)bulletinType.Bollettino896 && b.bulletinType != (int)bulletinType.PagoPA)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo BulletinType errato - Bollettino";
            }


            return ctrl;
        }

        public static ControlloDestinatario verificaDestinatarioTelegramma(NamesTelegramDto recipient)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();
            ListaControlloDestinatari l = new ListaControlloDestinatari();

            NamesTelegramDto Destinatario = recipient;
            NamesDto DestinatarioDto = new NamesDto();
            ControlloDestinatario ctrl = new ControlloDestinatario();
            ctrl.Destinatario = Mapper.Map(Destinatario, DestinatarioDto);

            string Cap = Destinatario.cap.Replace(" ", "");
            var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
            int i = comune.Count();

            // CONTROLLO CAP
            crt crt = verificaCap(Cap, i);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Destinatario";
            }

            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = verificaRagioneSociale(Destinatario.businessName, Destinatario.name, Destinatario.surname);
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Destinatario";
            }

            // CONTROLLO INDIRIZZO
            crt crtA = verificaIndirizzo(Destinatario.dug, Destinatario.address, Destinatario.houseNumber);
            if (!crtA.Valido)
            {
                ctrl.Valido = crtA.Valido;
                ctrl.Errore = crtA.Errore + " - Destinatario";
            }
            // CONTROLLO CITTA'
            crt crtC = verificaCitta(Destinatario.city);
            if (!crtC.Valido)
            {
                ctrl.Valido = crtC.Valido;
                ctrl.Errore = crtC.Errore + " - Destinatario";
            }

            // CONTROLLO PROVINCIA
            crt crtP = verificaProvincia(Destinatario.province);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Destinatario";
            }

            // CONTROLLO STATO
            crt crtS = verificaStato(Destinatario.state);
            if (!crtS.Valido)
            {
                ctrl.Valido = crtS.Valido;
                ctrl.Errore = crtS.Errore + " - Destinatario";
            }

            return ctrl;
        }

        public static ControlloMittente verificaMittente(SenderDto sender)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);

            SenderDto s = sender;
            ControlloMittente ctrl = new ControlloMittente();
            ctrl.sender = sender;

            string Cap = sender.cap.Replace(" ", "");
            var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
            int i = comune.Count();

            // CONTROLLO CAP
            crt crt = verificaCap(Cap, i);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Mittente";
            }

            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = verificaRagioneSociale(s.businessName, s.name, s.surname);
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Mittente";
            }

            // CONTROLLO INDIRIZZO
            crt crtA = verificaIndirizzo(s.dug, s.address, s.houseNumber);
            if (!crtA.Valido)
            {
                ctrl.Valido = crtA.Valido;
                ctrl.Errore = crtA.Errore + " - Mittente";
            }

            // CONTROLLO CITTA'
            crt crtC = verificaCitta(s.city);
            if (!crtC.Valido)
            {
                ctrl.Valido = crtC.Valido;
                ctrl.Errore = crtC.Errore + " - Mittente";
            }

            // CONTROLLO PROVINCIA
            crt crtP = verificaProvincia(s.province);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Mittente";
            }

            // CONTROLLO STATO
            crt crtS = verificaStato(s.state);
            if (!crtS.Valido)
            {
                ctrl.Valido = crtS.Valido;
                ctrl.Errore = crtS.Errore + " - Mittente";
            }

            return ctrl;
        }

        public static string creaPdf(byte[] b, string path)
        {
            var fileName = DateTime.Now.Ticks.ToString() + ".pdf";
            File.WriteAllBytes(HttpRuntime.AppDomainAppPath + path + fileName, b);
            return fileName;
        }

        public static byte[] getByteFromPdf()
        {
            byte[] b = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/public/ewt/test.pdf"));
            return b;
        }

        public static string Mid(string s, int a)

        {
            var l = s.Length;
            string temp = s.Substring(a - 1, l);
            return temp;
        }

        public static byte[] getByte()
        {
            byte[] array = null;
            long bytes1 = GC.GetTotalMemory(false);
            array = new byte[1000 * 1000 * 3];
            array[0] = 0;
            long bytes2 = GC.GetTotalMemory(false);
            return array;

        }

        public static decimal GetVat(decimal totale)
        {
            decimal q = Decimal.Divide(vat, 100);
            decimal f = 1 + q;
            decimal netPrice = Math.Round(totale / f, 2);
            return totale - netPrice;
        }

        public static Totals GeneraImportoStatico()
        {
            var t = new Totals();
            decimal tot = random.Next(1, 3);
            tot = tot + (decimal)random.NextDouble();
            decimal vat = GetVat(tot);
            t.ImportoTotale = (double)tot;
            t.ImportoIva = (double)vat;
            t.ImportoNetto = (double)(tot - vat);
            return t;
        }

        public static Totals GeneraImportoDaPrezzo(double price)
        {
            var t = new Totals();
            decimal q = Decimal.Divide(vat, 100);
            double vatPrice = price * Convert.ToDouble(q);

            t.ImportoTotale = Convert.ToDouble(price + vatPrice);
            t.ImportoIva = (double)vatPrice;
            t.ImportoNetto = (double)(price);
            return t;
        }

        public static string GeneraOrdineStatico()
        {
            Guid t = Guid.NewGuid();
            return t.ToString();
        }

        public static string GeneraNumeroRaccomandataStatico()
        {
            int tot = random.Next(600000000, 699999999);
            return tot.ToString();
        }

        public static string GeneraNumeroLetteraStatico()
        {
            int tot = random.Next(100000000, 199999999);
            return tot.ToString();
        }

        public static Prices GetFilePrice(operationType type, PdfDocument document, bool colori, string tipoPosta = "Posta4")
        {
            decimal totale = 0;

            if (document.Pages.Count == 1)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.49);
                        else
                            totale = Convert.ToDecimal(2.32);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(1.24);
                        else
                            totale = Convert.ToDecimal(1.07);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.00);
                    else
                        totale = Convert.ToDecimal(3.70);
            }

            if (document.Pages.Count == 2)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.60);
                        else
                            totale = Convert.ToDecimal(2.40);

                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(1.35);
                        else
                            totale = Convert.ToDecimal(1.15);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.15);
                    else
                        totale = Convert.ToDecimal(3.80);
            }

            if (document.Pages.Count == 3)
            {
                if (tipoPosta == "Posta1")
                {
                    if (colori)
                        totale = Convert.ToDecimal(2.69);
                    else
                        totale = Convert.ToDecimal(2.46);
                }
                else
                {
                    if (colori)
                        totale = Convert.ToDecimal(1.44);
                    else
                        totale = Convert.ToDecimal(1.21);
                }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.30);
                    else
                        totale = Convert.ToDecimal(3.90);
            }

            if (document.Pages.Count == 4 || document.Pages.Count == 5)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.81);
                        else
                            totale = Convert.ToDecimal(2.55);

                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.66);
                        else
                            totale = Convert.ToDecimal(2.40);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.35);
                    else
                        totale = Convert.ToDecimal(5.05);
            }

            if (document.Pages.Count == 6 || document.Pages.Count == 7 || document.Pages.Count == 8 || document.Pages.Count == 9)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.03);
                        else
                            totale = Convert.ToDecimal(2.67);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.88);
                        else
                            totale = Convert.ToDecimal(2.52);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.70);
                    else
                        totale = Convert.ToDecimal(5.10);
            }

            if (document.Pages.Count == 10 || document.Pages.Count == 11 || document.Pages.Count == 12 || document.Pages.Count == 13)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.33);
                        else
                            totale = Convert.ToDecimal(2.85);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.18);
                        else
                            totale = Convert.ToDecimal(2.70);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.85);
                    else
                        totale = Convert.ToDecimal(5.15);
            }

            if (document.Pages.Count > 13)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.78);
                        else
                            totale = Convert.ToDecimal(3.15);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.63);
                        else
                            totale = Convert.ToDecimal(3.00);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(6.00);
                    else
                        totale = Convert.ToDecimal(5.20);
            }

            document.Close();

            var p = new Prices();
            if (type == operationType.LOL)
            {
                decimal vat = GetVat(totale);
                p.vatPrice = vat;
                p.totalPrice = Convert.ToDecimal(totale);
                p.price = Convert.ToDecimal(totale) - vat;
            };
            if (type == operationType.ROL)
            {
                p.vatPrice = 0;
                p.totalPrice = Convert.ToDecimal(totale);
                p.price = Convert.ToDecimal(totale);
            }

            return p;
        }
        public static Prices GetPriceVol(operationType type, ServiceVol.CodiceDocumento codice)
        {
            double price = 0;
            //TEST
            switch (codice)
            {
                case ServiceVol.CodiceDocumento.VISO:
                    price = 6;
                    break;

                case ServiceVol.CodiceDocumento.VISS:
                    price = 7;
                    break;

                case ServiceVol.CodiceDocumento.SCPE:
                    price = 2.5;
                    break;

                case ServiceVol.CodiceDocumento.SCSO:
                    price = 3;
                    break;

                case ServiceVol.CodiceDocumento.SCSC:
                    price = 3.5;
                    break;

                case ServiceVol.CodiceDocumento.RIPR:
                    price = 4;
                    break;

                case ServiceVol.CodiceDocumento.BICM:
                    price = 8;
                    break;

                case ServiceVol.CodiceDocumento.FASC:
                    price = 11;
                    break;

                case ServiceVol.CodiceDocumento.TRSF:
                    price = 6;
                    break;

                case ServiceVol.CodiceDocumento.CRIM:
                    price = 9;
                    break;

                case ServiceVol.CodiceDocumento.CRIS:
                    price = 10.7;
                    break;

                case ServiceVol.CodiceDocumento.CRIA:
                    price = 9;
                    break;

                case ServiceVol.CodiceDocumento.CART:
                    price = 12;
                    break;

                case ServiceVol.CodiceDocumento.SOST:
                    price = 9;
                    break;

            }

            Totals t = GeneraImportoDaPrezzo(price);
            var p = new Prices()
            {
                vatPrice = Convert.ToDecimal(t.ImportoIva),
                totalPrice = Convert.ToDecimal(t.ImportoTotale),
                price = Convert.ToDecimal(price)
            };

            return p;
        }

        public static Prices GetFilePriceSpecialFormat(operationType type, PdfDocument document, bool colori, string tipoPosta = "Posta4")
        {
            decimal totale = 0;

            if (document.Pages.Count == 1)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.49);
                        else
                            totale = Convert.ToDecimal(2.32);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(1.24);
                        else
                            totale = Convert.ToDecimal(1.07);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.00);
                    else
                        totale = Convert.ToDecimal(3.70);
            }

            if (document.Pages.Count == 2)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.60);
                        else
                            totale = Convert.ToDecimal(2.40);

                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(1.35);
                        else
                            totale = Convert.ToDecimal(1.15);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.15);
                    else
                        totale = Convert.ToDecimal(3.80);
            }

            if (document.Pages.Count == 3)
            {
                if (tipoPosta == "Posta1")
                {
                    if (colori)
                        totale = Convert.ToDecimal(2.69);
                    else
                        totale = Convert.ToDecimal(2.46);
                }
                else
                {
                    if (colori)
                        totale = Convert.ToDecimal(1.44);
                    else
                        totale = Convert.ToDecimal(1.21);
                }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.30);
                    else
                        totale = Convert.ToDecimal(3.90);
            }

            if (document.Pages.Count == 4 || document.Pages.Count == 5)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.81);
                        else
                            totale = Convert.ToDecimal(2.55);

                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.66);
                        else
                            totale = Convert.ToDecimal(2.40);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.35);
                    else
                        totale = Convert.ToDecimal(5.05);
            }

            if (document.Pages.Count == 6 || document.Pages.Count == 7 || document.Pages.Count == 8 || document.Pages.Count == 9)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.03);
                        else
                            totale = Convert.ToDecimal(2.67);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(2.88);
                        else
                            totale = Convert.ToDecimal(2.52);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.70);
                    else
                        totale = Convert.ToDecimal(5.10);
            }

            if (document.Pages.Count == 10 || document.Pages.Count == 11 || document.Pages.Count == 12 || document.Pages.Count == 13)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.33);
                        else
                            totale = Convert.ToDecimal(2.85);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.18);
                        else
                            totale = Convert.ToDecimal(2.70);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.85);
                    else
                        totale = Convert.ToDecimal(5.15);
            }

            if (document.Pages.Count > 13)
            {
                if (type == operationType.LOL)
                    if (tipoPosta == "Posta1")
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.78);
                        else
                            totale = Convert.ToDecimal(3.15);
                    }
                    else
                    {
                        if (colori)
                            totale = Convert.ToDecimal(3.63);
                        else
                            totale = Convert.ToDecimal(3.00);
                    }

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(6.00);
                    else
                        totale = Convert.ToDecimal(5.20);
            }

            document.Close();

            var p = new Prices();
            if (type == operationType.LOL)
            {
                decimal vat = GetVat(totale);
                p.vatPrice = vat;
                p.totalPrice = Convert.ToDecimal(totale);
                p.price = Convert.ToDecimal(totale) - vat;
            };
            if (type == operationType.ROL)
            {
                p.vatPrice = 0;
                p.totalPrice = Convert.ToDecimal(totale);
                p.price = Convert.ToDecimal(totale);
            }

            return p;
        }
        public static void RemoveFileFolder(string SourcePath)
        {
            try
            {
                string[] filePaths = Directory.GetFiles(SourcePath);
                string[] dirPaths = Directory.GetDirectories(SourcePath);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
                foreach (string dirPath in dirPaths)
                {
                    string[] fileFolders = Directory.GetFiles(dirPath);
                    foreach (string fileFolder in fileFolders)
                        File.Delete(fileFolder);
                    Directory.Delete(dirPath);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static List<MolColState> ListOfState()
        {
            var l = new List<MolColState>();

            var s = new MolColState()
            {
                identificativo = "D",
                descrizione = "In Conversione",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "B",
                descrizione = "Acquisita",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "K",
                descrizione = "Pronta per conferma",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "Y",
                descrizione = "Scartata – Non convertibile",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "N",
                descrizione = "Scartata – Conversione non disponibile",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "J",
                descrizione = "Scartata – Fogli in eccesso",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "U",
                descrizione = "Annullata da sistema",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "T",
                descrizione = "In conferma",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "E",
                descrizione = "Scartata – Ricevuta non disponibile",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "C",
                descrizione = "Confermata",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "F",
                descrizione = "Annullata da cliente – Stampa",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "L",
                descrizione = "Presa in carico Postel",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "V",
                descrizione = "Timeout Postel",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "Q",
                descrizione = "In postalizzazione",
                tipologia = "Transitorio",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "S",
                descrizione = "Postalizzato",
                tipologia = "Definitivo",
                state = true
            };
            l.Add(s);

            s = new MolColState()
            {
                identificativo = "W",
                descrizione = "Errore nella stampa dei destinatari",
                tipologia = "Definitivo",
                state = false
            };
            l.Add(s);

            return l;
        }

    }
}