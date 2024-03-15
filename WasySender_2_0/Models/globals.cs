using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mail;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using WasySender_2_0.DataModel;
using GemBox.Document;
using Ionic.Zip;
using System.IO.Compression;
using System.Drawing;
using GemBox.Pdf;
using GemBox.Pdf.Content;
using tessnet2;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using IronOcr;
using System.Dynamic;
using System.Collections;
using System.Text;
using Oauth0.Models;

namespace WasySender_2_0.Models
{
    public class Globals
    {
        public static string staticUrl = "https://app.easysender.it/";
        public static string apiUri = "http://localhost:5001/api/";
        //public static string apiUri = "https://apinewtest.easysender.it/api/";
        //public static string apiUriTest = "https://apinewtest.easysender.it";

        //public static string staticUrl = "http://localhost:59447/";
        //public static string apiUri = "http://localhost:5000/api/";
        public static string downladFile = "https://private.easysender.it/private-files/";

        public static int limiteMaxPagine = 18;
        public static long limiteMaxPeso = 5000; 
        public static double incrPercSMS = 0.1;
        public static double minimumPriceSms = 385.00;
        public static double vat = 0.22;


        //FTP
        public const string ftpUrl = "https://ewt.group";
        public const string ftpOutputFolder = "/special/Output/";
        public const string ftpInputFolder = "/special/Input/";
        public const string usernameFtp = "mewthici";
        public const string passwordFtp = "WykbvJjdM9wUc9";

        //Oauth0
        public const string prefixRandstad = "@randstad.it;@ext.randstad.it;@intempolavoro.it;@randstadrisesmart.it";

        //EMAIL
        public static string mittente = "";
        public static string destinatarioAdmin = "";

        public static async Task<HttpResponseMessage> HttpClientSend(string type, string url, bool userAreaTest, object o = null)
        {
            var client = new HttpClient();
            client.Timeout = System.Threading.Timeout.InfiniteTimeSpan;
            client.BaseAddress = new Uri(apiUri);

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

        public static string NoFoto()
        {
            return "/images/noFoto.jpg";
        }

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

        public static async Task SetLogs(int logType, int userId, string description)
        {
            var log = new Logs()
            {
                logType = logType,
                userId = userId,
                description = description
            };

            await Globals.HttpClientSend("POST", "Logs/New", true, log);
        }

        public static Jwt GetJwtUser(string token)
        {
            var parts = token.Split('.');
            string partToConvert = parts[1];
            partToConvert = partToConvert.Replace('-', '+');
            partToConvert = partToConvert.Replace('_', '/');
            switch (partToConvert.Length % 4)
            {
                case 0:
                    break;
                case 2:
                    partToConvert += "==";
                    break;
                case 3:
                    partToConvert += "=";
                    break;
            }
            var partAsBytes = Convert.FromBase64String(partToConvert);
            var partAsUTF8String = Encoding.UTF8.GetString(partAsBytes, 0, partAsBytes.Count());

            var jwt = JsonConvert.DeserializeObject<Jwt>(partAsUTF8String);

            return jwt;
        }



        public static async Task<PricesSms> GetCampaignPriceSMS(int qta, bool areaTestUser)
        {

            decimal p = 0;
            decimal total = 0;
            decimal vat = 0;
            HttpResponseMessage get = await Globals.HttpClientSend("GET", "api/SmsLists/Price/" + qta, areaTestUser);
            if (get.IsSuccessStatusCode)
                p = await get.Content.ReadAsAsync<decimal>();

            //INCREMENTO PERCENTUALE
            p = (p * (decimal)incrPercSMS) + p;

            decimal netPrice = qta * p;
            if (netPrice < (decimal)minimumPriceSms)
                netPrice = (decimal)minimumPriceSms;

            vat = GetVatFromNetPrice(netPrice);
            total = netPrice + vat;

            PricesSms pr = new PricesSms()
            {
                untiPrice = p,
                netPrice = netPrice,
                vatPrice = vat,
                totalPrice = total
            };

            return pr;
        }
        public static string PacchiProductName(string number)
        {
            var str = "";
            switch (number.ToUpper())
            {
                case "APT000902":
                    str = "Standard";
                    break;
                case "APT000901":
                    str = "Standar Expressd";
                    break;
                case "APT000904":
                    str = "Internazionale Standar";
                    break;
                case "APT000903":
                    str = "Internazionale Express";
                    break;
            }

            return str;
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

        public static string GetCSVFromRemote(string url)
        {
            FileWebRequest req = (FileWebRequest)FileWebRequest.Create(url);
            FileWebResponse resp = (FileWebResponse)req.GetResponse();

            StreamReader sr = new StreamReader(resp.GetResponseStream());
            string results = sr.ReadToEnd();
            sr.Close();

            return results;
        }

        public static async Task<UploadListResponse> ReadCsvAndAddToList(string path, int listId, bool areaTestUser)
        {
            var r = new UploadListResponse();
            string fileList = Globals.GetCSV(path);
            string[] tempStr;

            tempStr = fileList.Split((char)10);
            int i = 0;

            List<NamesLists> lndm = new List<NamesLists>();
            int positionBusinessName = 0;
            int positionName = 0;
            int positionSurname = 0;
            int positionDug = 0;
            int positionAddress = 0;
            int positionHouseNumber = 0;
            int positionCap = 0;
            int positionCity = 0;
            int positionProvince = 0;
            int positionState = 0;
            int positionComplementAddress = 0;

            foreach (string row in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(row))
                {
                    string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                    if (column.Length < 11)
                    {
                        r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreLunghezzaCsv);
                        return r;
                    }

                    if (i == 0)
                    {
                        for (var x=0; x < column.Length; x++)
                        {
                            switch (column[x].ToUpper())
                            {
                                case "RAGIONESOCIALE":
                                case "RAGIONE SOCIALE":
                                    positionBusinessName = x;
                                    break;
                                case "NOME":
                                    positionName = x;
                                    break;
                                case "COGNOME":
                                    positionSurname = x;
                                    break;
                                case "DUG":
                                    positionDug = x;
                                    break;
                                case "INDIRIZZO":
                                    positionAddress = x;
                                    break;
                                case "NUMEROCIVICO":
                                case "NUMERO CIVICO":
                                    positionHouseNumber = x;
                                    break;
                                case "CAP":
                                    positionCap = x;
                                    break;
                                case "CITTA":
                                    positionCity = x;
                                    break;
                                case "PROVINCIA":
                                    positionProvince = x;
                                    break;
                                case "STATO":
                                    positionState = x;
                                    break;
                                case "COMPLETAMENTO INDIRIZZO":
                                case "COMPLETAMENTOINDIRIZZO":
                                    positionComplementAddress = x;
                                    break;
                            }
                        };
                    }
                    else
                    {
                        NamesLists ndm = new NamesLists();
                        ndm.listId = listId;
                        ndm.name = column[positionName];
                        ndm.surname = column[positionSurname];
                        ndm.businessName = column[positionBusinessName];
                        ndm.dug = column[positionDug];
                        ndm.address = column[positionAddress];
                        ndm.houseNumber = column[positionHouseNumber];
                        ndm.cap = column[positionCap];
                        ndm.city = column[positionCity];
                        ndm.province = column[positionProvince];
                        ndm.state = column[positionState];
                        ndm.complementAddress = column[positionComplementAddress];
                        lndm.Add(ndm);
                    }
                }
                i++;
            }

            HttpResponseMessage get = await Globals.HttpClientSend("POST", "/api/NamesLists/NewMultiple", areaTestUser, lndm);
            if (!get.IsSuccessStatusCode)
            {
                r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreGenerico);
                return r;
            }

            r.errors = 0;
            r.NamesLists = lndm;
            r.numberOfNames = lndm.Count();
            r.listId = listId;
            r.success = (lndm.Count() > 0 ? true : false);
            return r;
        }

        public static async Task<UploadListResponse> ReadCsvShowResults(string path, Users user, string sessionId, string logo)
        {
            var r = new UploadListResponse();
            try
            {
                string fileList = Globals.GetCSV(path);
                string[] tempStr;
                tempStr = fileList.Split((char)10);
                int i = 0;

                var comuni = Globals.GetComuniList();

                List<NamesLists> lndm = new List<NamesLists>();
                int positionBusinessName = 0;
                int positionName = 0;
                int positionSurname = 0;
                //int positionDug = 0;
                int positionAddress = 0;
                //int positionHouseNumber = 0;
                int positionCap = 0;
                int positionCity = 0;
                int positionProvince = 0;
                int positionState = 0;
                int positionFileName = 0;
                int errors = 0;
                int positionComplementAddress = 0;
                int positionComplementNames = 0;
                int positionCf = 0;
                int positionMobile = 0;
                int positionSms = 0;
                int positionTestoSms = 0;

                //ELIMINAZIONE VECCHI CONTROLLI
                HttpResponseMessage get = await HttpClientSend("GET", "TemporaryValidateTable/Delete?userId=" + user.id + "&sessionId=" + sessionId, user.areaTestUser);

                foreach (string row in tempStr)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(row))
                        {
                            string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                            var cl = column.Length;
                            if (column.Length < 12)
                            {
                                r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreLunghezzaCsv);
                                return r;
                            }

                            if (i == 0)
                            {
                                for (var x = 0; x < column.Length; x++)
                                {
                                    switch (column[x].ToUpper())
                                    {
                                        case "RAGIONESOCIALE":
                                        case "RAGIONE SOCIALE":
                                            positionBusinessName = x;
                                            break;
                                        case "NOME":
                                            positionName = x;
                                            break;
                                        case "COGNOME":
                                            positionSurname = x;
                                            break;
                                        //case "DUG":
                                        //    positionDug = x;
                                        //    break;
                                        case "INDIRIZZO":
                                            positionAddress = x;
                                            break;
                                        //case "NUMEROCIVICO":
                                        //case "NUMERO CIVICO":
                                        //    positionHouseNumber = x;
                                        //    break;
                                        case "CAP":
                                            positionCap = x;
                                            break;
                                        case "CITTA":
                                            positionCity = x;
                                            break;
                                        case "PROVINCIA":
                                            positionProvince = x;
                                            break;
                                        case "STATO":
                                            positionState = x;
                                            break;
                                        case "NOMEFILE":
                                        case "NOME FILE":
                                            positionFileName = x;
                                            break;
                                        case "COMPLETAMENTO INDIRIZZO":
                                        case "COMPLETAMENTOINDIRIZZO":
                                            positionComplementAddress = x;
                                            break;
                                        case "COMPLETAMENTO NOMINATIVO":
                                        case "COMPLETAMENTONOMINATIVO":
                                            positionComplementNames = x;
                                            break;
                                        case "CODICE FISCALE":
                                        case "CODICEFISCALE":
                                        case "CF":
                                            positionCf = x;
                                            break;
                                        case "TELEFONO":
                                        case "MOBILE":
                                        case "CELLULARE":
                                        case "CELL":
                                            positionMobile = x;
                                            break;
                                        case "SMS":
                                            positionSms = x;
                                            break;
                                        case "TESTOSMS":
                                            positionTestoSms = x;
                                            break;
                                    }
                                };
                            }
                            else
                            {
                                var comune = comuni.Where(c => c.cap == column[positionCap]).ToList();
                                var sms = column[positionSms];
                                bool sendSms = false;
                                if (sms.ToUpper() == "SI")
                                    sendSms = true;

                                NamesLists ndm = new NamesLists();
                                ndm.listId = 0;
                                ndm.name = column[positionName];
                                ndm.surname = column[positionSurname];
                                ndm.businessName = column[positionBusinessName];
                                //ndm.dug = column[positionDug];
                                ndm.address = column[positionAddress];
                                //ndm.houseNumber = column[positionHouseNumber];
                                ndm.cap = column[positionCap];
                                ndm.city = column[positionCity];
                                ndm.province = column[positionProvince];
                                ndm.state = column[positionState];
                                ndm.fileName = column[positionFileName];
                                ndm.complementAddress = column[positionComplementAddress];
                                ndm.complementNames = column[positionComplementNames];
                                ndm.fiscalCode = column[positionCf];
                                ndm.mobile = column[positionMobile];
                                ndm.valid = true;
                                ndm.sms = sendSms;
                                ndm.testoSms = column[positionTestoSms];
                                ndm.logo = logo;

                                var res = CheckRecipient.verificaDestinatario(ndm, comune, true);
                                if (!res.Valido)
                                {
                                    errors++;
                                    ndm.valid = false;
                                    ndm.errorMessage = res.Errore;
                                }
                                else
                                {
                                    ndm.errorMessage = res.Errore;
                                }

                                var t = new TemporaryValidateTable()
                                {
                                    id = Guid.NewGuid(),
                                    sessionId = sessionId,
                                    userId = user.id,
                                    valid = res.Valido,
                                    totalNames = tempStr.Count()
                                };

                                //INSERIMENTO NUOVO CONTROLLO
                                get = await HttpClientSend("POST", "TemporaryValidateTable/New", user.areaTestUser, t);

                                lndm.Add(ndm);
                            }
                            i++;
                        }
                    }
                    catch (Exception e)
                    {
                        var ee = e;
                    }
                }
                r.errors = errors;
                r.NamesLists = lndm;
                r.numberOfNames = lndm.Count();
                r.listId = 0;
                r.success = (lndm.Count() > 0 ? true : false);
            }
            catch (Exception e)
            {
                var t = e;
            }

            return r;
        }

        public static async Task<UploadListResponse> ReadCsvShowResultsMassive(string path, Users user, string sessionId, string logo, string dir, bool
            fronteRetro)
        {
            var r = new UploadListResponse();
            try
            {
                string fileList = Globals.GetCSV(path);
                string[] tempStr;
                tempStr = fileList.Split((char)10);
                int i = 0;

                var comuni = Globals.GetComuniList();

                List<NamesLists> lndm = new List<NamesLists>();
                int positionBusinessName = 0;
                int positionName = 0;
                int positionSurname = 0;
                //int positionDug = 0;
                int positionAddress = 0;
                //int positionHouseNumber = 0;
                int positionCap = 0;
                int positionCity = 0;
                int positionProvince = 0;
                int positionState = 0;
                int positionFileName = 0;
                int errors = 0;
                int positionComplementAddress = 0;
                int positionComplementNames = 0;
                int positionCf = 0;
                int positionMobile = 0;
                int positionSms = 0;
                int positionTestoSms = 0;

                //ELIMINAZIONE VECCHI CONTROLLI
                HttpResponseMessage get = await HttpClientSend("GET", "TemporaryValidateTable/Delete?userId=" + user.id + "&sessionId=" + sessionId, user.areaTestUser);

                foreach (string row in tempStr)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(row))
                        {
                            string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                            var cl = column.Length;
                            if (column.Length < 12)
                            {
                                r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreLunghezzaCsv);
                                return r;
                            }

                            if (i == 0)
                            {
                                for (var x = 0; x < column.Length; x++)
                                {
                                    switch (column[x].ToUpper())
                                    {
                                        case "RAGIONESOCIALE":
                                        case "RAGIONE SOCIALE":
                                            positionBusinessName = x;
                                            break;
                                        case "NOME":
                                            positionName = x;
                                            break;
                                        case "COGNOME":
                                            positionSurname = x;
                                            break;
                                        //case "DUG":
                                        //    positionDug = x;
                                        //    break;
                                        case "INDIRIZZO":
                                            positionAddress = x;
                                            break;
                                        //case "NUMEROCIVICO":
                                        //case "NUMERO CIVICO":
                                        //    positionHouseNumber = x;
                                        //    break;
                                        case "CAP":
                                            positionCap = x;
                                            break;
                                        case "CITTA":
                                            positionCity = x;
                                            break;
                                        case "PROVINCIA":
                                            positionProvince = x;
                                            break;
                                        case "STATO":
                                            positionState = x;
                                            break;
                                        case "NOMEFILE":
                                        case "NOME FILE":
                                            positionFileName = x;
                                            break;
                                        case "COMPLETAMENTO INDIRIZZO":
                                        case "COMPLETAMENTOINDIRIZZO":
                                            positionComplementAddress = x;
                                            break;
                                        case "COMPLETAMENTO NOMINATIVO":
                                        case "COMPLETAMENTONOMINATIVO":
                                            positionComplementNames = x;
                                            break;
                                        case "CODICE FISCALE":
                                        case "CODICEFISCALE":
                                        case "CF":
                                            positionCf = x;
                                            break;
                                        case "TELEFONO":
                                        case "MOBILE":
                                        case "CELLULARE":
                                        case "CELL":
                                            positionMobile = x;
                                            break;
                                        case "SMS":
                                            positionSms = x;
                                            break;
                                        case "TESTOSMS":
                                            positionTestoSms = x;
                                            break;
                                    }
                                };
                            }
                            else
                            {
                                var comune = comuni.Where(c => c.cap == column[positionCap]).ToList();
                                var sms = column[positionSms];
                                bool sendSms = false;
                                if (sms.ToUpper() == "SI")
                                    sendSms = true;
                                dir = HttpContext.Current.Server.MapPath("/Public/MassiveImport/Zip/");
                                //dir = "C:/Users/GiulianoValente/Desktop/AccontoTari2022/";

                                NamesLists ndm = new NamesLists();
                                ndm.listId = 0;
                                ndm.name = column[positionName];
                                ndm.surname = column[positionSurname];
                                ndm.businessName = column[positionBusinessName];
                                //ndm.dug = column[positionDug];
                                ndm.address = column[positionAddress];
                                //ndm.houseNumber = column[positionHouseNumber];
                                ndm.cap = column[positionCap];
                                ndm.city = column[positionCity];
                                ndm.province = column[positionProvince];
                                ndm.state = column[positionState];
                                ndm.fileName = dir + column[positionFileName];
                                ndm.complementAddress = column[positionComplementAddress];
                                ndm.complementNames = column[positionComplementNames];
                                ndm.fiscalCode = column[positionCf];
                                ndm.mobile = column[positionMobile];
                                ndm.valid = true;
                                ndm.sms = sendSms;
                                ndm.testoSms = column[positionTestoSms];
                                ndm.logo = logo;

                                var res = CheckRecipient.verificaDestinatario(ndm, comune, true);
                                if (!res.Valido)
                                {
                                    errors++;
                                    ndm.valid = false;
                                    ndm.errorMessage = res.Errore;
                                }
                                else
                                {
                                    //CONVALIDA ASSOCIAZIONE FILE
                                    if (!System.IO.File.Exists(ndm.fileName))
                                    {
                                        res.Errore = "Nessun file corrispondente al nominativo.";
                                        res.Valido = false;
                                    }
                                    

                                    if (!res.Valido)
                                    {
                                        errors++;
                                        ndm.valid = false;
                                        ndm.errorMessage = res.Errore;
                                    }
                                    else
                                    {
                                        ndm.errorMessage = res.Errore;
                                    }

                                }

                                var t = new TemporaryValidateTable()
                                {
                                    id = Guid.NewGuid(),
                                    sessionId = sessionId,
                                    userId = user.id,
                                    valid = res.Valido,
                                    totalNames = tempStr.Count()
                                };

                                //INSERIMENTO NUOVO CONTROLLO
                                get = await HttpClientSend("POST", "TemporaryValidateTable/New", user.areaTestUser, t);

                                lndm.Add(ndm);
                            }
                            i++;
                        }
                    }
                    catch (Exception e)
                    {
                        var ee = e;
                    }
                }
                r.errors = errors;
                r.NamesLists = lndm;
                r.numberOfNames = lndm.Count();
                r.listId = 0;
                r.success = (lndm.Count() > 0 ? true : false);
            }
            catch (Exception e)
            {
                var t = e;
            }

            return r;
        }
        public static async Task<UploadListResponse> ReadCsvShowResultsPacchi(string path, Users user, string sessionId, bool senderFromContract, string product)
        {
            var r = new UploadListResponse();
            try
            {
                string fileList = Globals.GetCSV(path);
                string[] tempStr;
                tempStr = fileList.Split((char)10);
                int i = 0;

                var comuni = Globals.GetComuniList();

                List<NamesLists> lndm = new List<NamesLists>();
                int positionBusinessName = 0;
                int positionName = 0;
                int positionSurname = 0;
                int positionAddress = 0;
                int positionCap = 0;
                int positionCity = 0;
                int positionProvince = 0;
                int positionState = 0;
                int positionFileName = 0;
                int errors = 0;
                int positionComplementAddress = 0;
                int positionComplementNames = 0;
                int positionCf = 0;
                int positionMobile = 0;


                int positionShipmentDate = 0;
                int positionWeight = 0;
                int positionHeight = 0;
                int positionLength = 0;
                int positionWidth = 0;
                int positionContentText = 0;
                int positionContrassegno = 0;
                int positionRitiroAlPiano = 0;
                int positionRitornoAlMittente = 0;
                int positionAssicurazione = 0;


                //ELIMINAZIONE VECCHI CONTROLLI
                HttpResponseMessage get = await HttpClientSend("GET", "TemporaryValidateTable/Delete?userId=" + user.id + "&sessionId=" + sessionId, user.areaTestUser);

                foreach (string row in tempStr)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(row))
                        {
                            string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                            var cl = column.Length;
                            if (column.Length < 17)
                            {
                                r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreLunghezzaCsv);
                                return r;
                            }

                            if (i == 0)
                            {
                                for (var x = 0; x < column.Length; x++)
                                {
                                    switch (column[x].ToUpper())
                                    {
                                        case "RAGIONESOCIALE":
                                        case "RAGIONE SOCIALE":
                                            positionBusinessName = x;
                                            break;
                                        case "NOME":
                                            positionName = x;
                                            break;
                                        case "COGNOME":
                                            positionSurname = x;
                                            break;
                                        case "INDIRIZZO":
                                            positionAddress = x;
                                            break;
                                        case "CAP":
                                            positionCap = x;
                                            break;
                                        case "CITTA":
                                            positionCity = x;
                                            break;
                                        case "PROVINCIA":
                                            positionProvince = x;
                                            break;
                                        case "STATO":
                                            positionState = x;
                                            break;
                                        case "NOMEFILE":
                                        case "NOME FILE":
                                            positionFileName = x;
                                            break;
                                        case "COMPLETAMENTO INDIRIZZO":
                                        case "COMPLETAMENTOINDIRIZZO":
                                            positionComplementAddress = x;
                                            break;
                                        case "COMPLETAMENTO NOMINATIVO":
                                        case "COMPLETAMENTONOMINATIVO":
                                            positionComplementNames = x;
                                            break;
                                        case "CODICE FISCALE":
                                        case "CODICEFISCALE":
                                        case "CF":
                                            positionCf = x;
                                            break;
                                        case "TELEFONO":
                                        case "MOBILE":
                                        case "CELLULARE":
                                        case "CELL":
                                            positionMobile = x;
                                            break;
                                        case "SHIPMENTDATE":
                                        case "DATAINVIO":
                                            positionShipmentDate = x;
                                            break;
                                        case "WEIGHT":
                                        case "PESO":
                                            positionWeight = x;
                                            break;
                                        case "HEIGHT":
                                        case "ALTEZZA":
                                            positionHeight = x;
                                            break;
                                        case "LENGTH":
                                        case "LUNGHEZZA":
                                            positionLength = x;
                                            break;
                                        case "WIDTH":
                                        case "PROFONDITA":
                                            positionWidth = x;
                                            break;
                                        case "CONTENTTEXT":
                                        case "CONTENUTO":
                                            positionContentText = x;
                                            break;
                                        case "CONTRASSEGNO":
                                            positionContrassegno = x;
                                            break;
                                        case "RITORNOALMITTENTE":
                                            positionRitornoAlMittente = x;
                                            break;
                                        case "ASSICURAZIONE":
                                            positionAssicurazione = x;
                                            break;
                                    }
                                };
                            }
                            else
                            {
                                var comune = comuni.Where(c => c.cap == column[positionCap]).ToList();

                                NamesLists ndm = new NamesLists();
                                ndm.listId = 0;
                                ndm.name = column[positionName];
                                ndm.surname = column[positionSurname];
                                ndm.businessName = column[positionBusinessName];
                                ndm.address = column[positionAddress];
                                ndm.cap = column[positionCap];
                                ndm.city = column[positionCity];
                                ndm.province = column[positionProvince];
                                ndm.state = column[positionState];
                                ndm.fileName = column[positionFileName];
                                ndm.complementAddress = column[positionComplementAddress];
                                ndm.complementNames = column[positionComplementNames];
                                ndm.fiscalCode = column[positionCf];
                                ndm.mobile = column[positionMobile];
                                ndm.shipmentDate = ConvertToDateTime(column[positionShipmentDate]);
                                ndm.weight = Convert.ToInt32(column[positionWeight]);
                                ndm.height = Convert.ToInt32(column[positionHeight]);
                                ndm.length = Convert.ToInt32(column[positionLength]);
                                ndm.width = Convert.ToInt32(column[positionWidth]);
                                ndm.contentText = column[positionContentText];
                                ndm.additionalServices = GeneraAdditionalServices(column[positionContrassegno], column[positionRitornoAlMittente], column[positionAssicurazione]);
                                ndm.senderFromContract = senderFromContract;
                                ndm.contrassegno = column[positionContrassegno];
                                ndm.ritornoAlMittente = column[positionRitornoAlMittente];
                                ndm.assicurazione = column[positionAssicurazione];
                                ndm.product = product;

                                ndm.valid = true;
                                var res = CheckRecipient.verificaDestinatarioPacchi(ndm, comune, true);
                                if (!res.Valido)
                                {
                                    errors++;
                                    ndm.valid = false;
                                    ndm.errorMessage = res.Errore;
                                }
                                else
                                {
                                    ndm.errorMessage = res.Errore;
                                }

                                var t = new TemporaryValidateTable()
                                {
                                    id = Guid.NewGuid(),
                                    sessionId = sessionId,
                                    userId = user.id,
                                    valid = res.Valido,
                                    totalNames = tempStr.Count()
                                };

                                //INSERIMENTO NUOVO CONTROLLO
                                get = await HttpClientSend("POST", "TemporaryValidateTable/New", user.areaTestUser, t);

                                lndm.Add(ndm);
                            }
                            i++;
                        }
                    }
                    catch (Exception e)
                    {
                        r.errorMessage = e.Message.ToString();
                    }
                }
                r.errors = errors;
                r.NamesLists = lndm;
                r.numberOfNames = lndm.Count();
                r.listId = 0;
                r.success = (lndm.Count() > 0 ? true : false);
            }
            catch (Exception e)
            {
                var t = e;
            }

            return r;
        }

        public static DateTime ConvertToDateTime(string d)
        {
            var p = d.Split('/');
            return Convert.ToDateTime(p[0] + "/" + p[1] + "/" + p[2]);
        }

        public static string GeneraAdditionalServices(string contrassegno, string ritornoAlMittente, string assicurazione)
        {
            var d = new Dictionary<string, Dictionary<string, string>>();
            if (contrassegno != "")
            {
                contrassegno = contrassegno.Replace(",", ".");
                if (contrassegno.IndexOf('.') <= 0)
                    contrassegno += ".00";

                var dict = new Dictionary<string, string>();
                dict.Add("amount", contrassegno);
                dict.Add("paymentMode", "ABM");
                d.Add("APT000918", dict);
            }
            if (ritornoAlMittente.ToUpper() == "SI")
            {
                var dict = new Dictionary<string, string>();
                dict.Add("return", "A");
                d.Add("APT000930", dict);
            }
            if (assicurazione != "" )
            {
                assicurazione = assicurazione.Replace(",", ".");
                if (assicurazione.IndexOf('.') <= 0)
                    assicurazione += ".00";

                var dict = new Dictionary<string, string>();
                dict.Add("amount", assicurazione);
                d.Add("APT000919", dict);
            }

            if (d.Count() > 0)
                return JsonConvert.SerializeObject(d);

            return "";
        }
        public static async Task<UploadListResponse> ReadCsvVisureShowResults(string path, Users user, string sessionId)
        {
            var r = new UploadListResponse();
            try
            {
                string fileList = Globals.GetCSV(path);
                string[] tempStr;
                tempStr = fileList.Split((char)10);
                int i = 0;

                List<NamesLists> lndm = new List<NamesLists>();
                int positionBusinessName = 0;
                int positionCCIAA = 0;
                int positionFiscalCode = 0;
                int positionNREA = 0;
                int errors = 0;

                //ELIMINAZIONE VECCHI CONTROLLI
                HttpResponseMessage get = await HttpClientSend("GET", "TemporaryValidateTable/Delete?userId=" + user.id + "&sessionId=" + sessionId, user.areaTestUser);

                foreach (string row in tempStr)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(row))
                        {
                            string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                            var cl = column.Length;
                            if (column.Length < 4)
                            {
                                r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreLunghezzaCsv);
                                return r;
                            }

                            if (i == 0)
                            {
                                for (var x = 0; x < column.Length; x++)
                                {
                                    switch (column[x].ToUpper())
                                    {
                                        case "RAGIONESOCIALE":
                                        case "RAGIONE SOCIALE":
                                            positionBusinessName = x;
                                            break;
                                        case "CCIAA":
                                            positionCCIAA = x;
                                            break;
                                        case "CODICE FISCALE / PARTITA IVA":
                                        case "CODICEFISCALE/PARTITAIVA":
                                        case "CF/PIVA":
                                            positionFiscalCode = x;
                                            break;
                                        case "NUMERO REA":
                                        case "NUMEROREA":
                                        case "NREA":
                                            positionNREA = x;
                                            break;
                                    }
                                };
                            }
                            else
                            {
                                if(column[positionBusinessName] != "" && column[positionCCIAA] != "" && column[positionFiscalCode] != "") { 
                                    NamesLists ndm = new NamesLists();
                                    ndm.listId = 0;
                                    ndm.name = "";
                                    ndm.surname = "";
                                    ndm.businessName = column[positionBusinessName];
                                    ndm.address = "";
                                    ndm.province = column[positionCCIAA];
                                    ndm.state = "";
                                    ndm.fileName = "";
                                    ndm.complementAddress = "";
                                    ndm.complementNames = "";
                                    ndm.fiscalCode = column[positionFiscalCode];
                                    ndm.mobile = "";
                                    ndm.valid = true;
                                    ndm.NREA = column[positionNREA];

                                    var res = await CheckRecipient.verificaDestinatarioVisure(ndm);
                                    if (!res.Valido)
                                    {
                                        errors++;
                                        ndm.valid = false;
                                        ndm.errorMessage = res.Errore;
                                    }
                                    else
                                    {
                                        ndm.errorMessage = res.Errore;
                                    }

                                    var t = new TemporaryValidateTable()
                                    {
                                        id = Guid.NewGuid(),
                                        sessionId = sessionId,
                                        userId = user.id,
                                        valid = res.Valido,
                                        totalNames = tempStr.Count()
                                    };

                                    //INSERIMENTO NUOVO CONTROLLO
                                    get = await HttpClientSend("POST", "TemporaryValidateTable/New", user.areaTestUser, t);

                                    lndm.Add(ndm);
                                }
                            }
                            i++;
                        }
                    }
                    catch (Exception e)
                    {
                        var ee = e;
                    }
                }
                r.errors = errors;
                r.NamesLists = lndm;
                r.numberOfNames = lndm.Count();
                r.listId = 0;
                r.success = (lndm.Count() > 0 ? true : false);
            }
            catch (Exception e)
            {
                var t = e;
            }

            return r;
        }
        public static async Task<UploadListResponse> ReadCsvBollettiniShowResults(string path, Users user, string sessionId, string logo)
        {
            var comuni = Globals.GetComuniList();

            var r = new UploadListResponse();
            string fileList = Globals.GetCSV(path);
            string[] tempStr;

            tempStr = fileList.Split((char)10);
            int i = 0;

            List<NamesLists> lndm = new List<NamesLists>();
            List<Bulletins> lb = new List<Bulletins>();
            int positionBusinessName = 0;
            int positionName = 0;
            int positionSurname = 0;
            //int positionDug = 0;
            int positionAddress = 0;
            //int positionHouseNumber = 0;
            int positionCap = 0;
            int positionCity = 0;
            int positionProvince = 0;
            int positionState = 0;
            int positionFileName = 0;
            int positionComplementNames = 0;
            int positionComplementAddress = 0;
            int positionCf = 0;
            int positionMobile = 0;
            int errors = 0;

            int positionNumeroContoCorrente = 0;
            int positionIntestatoA = 0;
            int positionCodiceCliente = 0;
            int positionImportoEuro = 0;
            int positionEseguitoDaNominativo = 0;
            int positionEseguitoDaIndirizzo = 0;
            int positionEseguitoDaCAP = 0;
            int positionEseguitoDaLocalita = 0;
            int positionCausale = 0;
            int positionAnno = 0;
            int positionIBAN = 0;

            int positionScadenza = 0;
            int positionCBILL = 0;
            int positionCodiciAvvisi = 0;

            int positionSms = 0;
            int positionTestoSms = 0;

            //ELIMINAZIONE VECCHI CONTROLLI
            HttpResponseMessage get = await HttpClientSend("GET", "TemporaryValidateTable/Delete?userId=" + user.id + "&sessionId=" + sessionId, user.areaTestUser);

            foreach (string row in tempStr)
            {
                if (!string.IsNullOrWhiteSpace(row))
                {
                    string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                    if (column.Length < 23)
                    {
                        r.errorMessage = EnumHelper<ErrorMessageUploadCsv>.GetDisplayValue(ErrorMessageUploadCsv.ErroreLunghezzaCsv);
                        return r;
                    }

                    if (i == 0)
                    {
                        for (var x = 0; x < column.Length; x++)
                        {
                            switch (column[x].ToUpper())
                            {
                                case "RAGIONESOCIALE":
                                case "RAGIONE SOCIALE":
                                    positionBusinessName = x;
                                    break;
                                case "NOME":
                                    positionName = x;
                                    break;
                                case "COGNOME":
                                    positionSurname = x;
                                    break;
                                //case "DUG":
                                //    positionDug = x;
                                //    break;
                                case "INDIRIZZO":
                                    positionAddress = x;
                                    break;
                                //case "NUMEROCIVICO":
                                //case "NUMERO CIVICO":
                                //    positionHouseNumber = x;
                                //    break;
                                case "CAP":
                                    positionCap = x;
                                    break;
                                case "CITTA":
                                    positionCity = x;
                                    break;
                                case "PROVINCIA":
                                    positionProvince = x;
                                    break;
                                case "STATO":
                                    positionState = x;
                                    break;
                                case "NOMEFILE":
                                    positionFileName = x;
                                    break;
                                case "NUMEROCONTOCORRENTE":
                                case "NUMERO CONTO CORRENTE":
                                    positionNumeroContoCorrente = x;
                                    break;
                                case "INTESTATOA":
                                case "INTESTATO A":
                                    positionIntestatoA = x;
                                    break;
                                case "CODICECLIENTE":
                                case "CODICE CLIENTE":
                                    positionCodiceCliente = x;
                                    break;
                                case "IMPORTOEURO":
                                case "IMPORTO EURO":
                                    positionImportoEuro = x;
                                    break;
                                case "ESEGUITODANOMINATIVO":
                                case "ESEGUITO DA NOMINATIVO":
                                    positionEseguitoDaNominativo = x;
                                    break;
                                case "ESEGUITODAINDIRIZZO":
                                case "ESEGUITO DA INDIRIZZO":
                                    positionEseguitoDaIndirizzo = x;
                                    break;
                                case "ESEGUITODACAP":
                                case "ESEGUITO DA CAP":
                                    positionEseguitoDaCAP = x;
                                    break;
                                case "ESEGUITODALOCALITA":
                                case "ESEGUITO DA LOCALITA":
                                    positionEseguitoDaLocalita = x;
                                    break;
                                case "CAUSALE":
                                    positionCausale = x;
                                    break;
                                case "ANNO":
                                    positionAnno = x;
                                    break;
                                case "COMPLETAMENTO INDIRIZZO":
                                case "COMPLETAMENTOINDIRIZZO":
                                    positionComplementAddress = x;
                                    break;
                                case "COMPLETAMENTO NOMINATIVO":
                                case "COMPLETAMENTONOMINATIVO":
                                    positionComplementNames = x;
                                    break;
                                case "CODICE FISCALE":
                                case "CODICEFISCALE":
                                case "CF":
                                    positionCf = x;
                                    break;
                                case "TELEFONO":
                                case "MOBILE":
                                case "CELLULARE":
                                case "CELL":
                                    positionMobile = x;
                                    break;
                                case "SCADENZA":
                                    positionScadenza = x;
                                    break;
                                case "CBILL":
                                    positionCBILL = x;
                                    break;
                                case "CODICIAVVISI":
                                    positionCodiciAvvisi = x;
                                    break;
                                case "IBAN":
                                    positionIBAN = x;
                                    break;

                                case "SMS":
                                    positionSms = x;
                                    break;
                                case "TESTOSMS":
                                    positionTestoSms = x;
                                    break;

                            }
                        };
                    }
                    else
                    {
                        var comune = comuni.Where(c => c.cap == column[positionCap]);
                        var sms = column[positionSms];
                        bool sendSms = false;
                        if (sms.ToUpper() == "SI")
                            sendSms = true;

                        NamesLists ndm = new NamesLists();
                        ndm.listId = 0;
                        ndm.name = column[positionName];
                        ndm.surname = column[positionSurname];
                        ndm.businessName = column[positionBusinessName];
                        //ndm.dug = column[positionDug];
                        ndm.address = column[positionAddress];
                        //ndm.houseNumber = column[positionHouseNumber];
                        ndm.cap = column[positionCap];
                        ndm.city = column[positionCity];
                        ndm.province = column[positionProvince];
                        ndm.state = column[positionState];
                        ndm.fileName = column[positionFileName];
                        ndm.complementAddress = column[positionComplementAddress];
                        ndm.complementNames = column[positionComplementNames];
                        ndm.fiscalCode = column[positionCf];
                        ndm.mobile = column[positionMobile];
                        ndm.valid = true;
                        ndm.sms = sendSms;
                        ndm.testoSms = column[positionTestoSms];
                        ndm.logo = logo;

                        var res = CheckRecipient.verificaDestinatario(ndm, comune, true);
                        if (!res.Valido)
                        {
                            errors++;
                            ndm.valid = false;
                            ndm.errorMessage = res.Errore;
                        }

                        //GENERAZIONE RANDOM CODICE CLIENTE
                        var CodiceCliente = column[positionCodiceCliente].Replace("cc", "").Replace("CC", "");
                        if (column[positionCodiceCliente].Replace("cc", "").Replace("CC", "") == "" && column[positionAnno] != "")
                        {
                            CodiceCliente = Globals.GetCodiceClienteBollettino(column[positionAnno], column[positionEseguitoDaCAP]);
                        }

                        //SE HA I CAMPI DEL PAGO PA NEL CSV, E' UN PAGO PA
                        int bulletinTypes = (int)bulletinType.Bollettino896;
                        bool pagoPA = false; 
                        if (positionScadenza > 0 && positionCBILL > 0 && positionCodiciAvvisi > 0) { 
                            bulletinTypes = (int)bulletinType.PagoPA;
                            pagoPA = true;
                        }

                        var b = new Bulletins()
                        {
                            NumeroContoCorrente = column[positionNumeroContoCorrente].Replace("cc", "").Replace("CC", ""),
                            IntestatoA = column[positionIntestatoA],
                            CodiceCliente = CodiceCliente,
                            ImportoEuro = Convert.ToDecimal(column[positionImportoEuro]),
                            EseguitoDaNominativo = column[positionEseguitoDaNominativo],
                            EseguitoDaIndirizzo = column[positionEseguitoDaIndirizzo],
                            EseguitoDaCAP = column[positionEseguitoDaCAP],
                            EseguitoDaLocalita = column[positionEseguitoDaLocalita],
                            Causale = column[positionCausale],
                            IBAN = column[positionIBAN],
                            BulletinType = bulletinTypes,
                            PagoPA = pagoPA
                        };

                        //SE E' UN PAGO PA AGGIUNGO I CAMPI
                        if (positionScadenza > 0 && positionCBILL > 0 && positionCodiciAvvisi > 0)
                        {
                            b.Scadenza = column[positionScadenza];
                            b.CBILL = column[positionCBILL];
                            b.CodiciAvvisi = column[positionCodiciAvvisi];
                        }

                        var resb = CheckBulletin.verificaBollettino(b);
                        if (!resb.Valido)
                            if(res.Valido)
                            {
                                errors++;
                                ndm.valid = false;
                                ndm.errorMessage = resb.Errore;
                            }

                        var t = new TemporaryValidateTable()
                        {
                            id = Guid.NewGuid(),
                            sessionId = sessionId,
                            userId = user.id,
                            valid = res.Valido,
                            totalNames = tempStr.Count()
                        };

                        //INSERIMENTO NUOVO CONTROLLO
                        get = await HttpClientSend("POST", "TemporaryValidateTable/New", user.areaTestUser, t);


                        lb.Add(b);
                        lndm.Add(ndm);
                    }
                    i++;
                }
            }
            r.errors = errors;
            r.NamesLists = lndm;
            r.Bulletins = lb;
            r.numberOfNames = lndm.Count();
            r.listId = 0;
            r.success = (lndm.Count() > 0 ? true : false);
            return r;
        }

        public static bool onlyNumbers(string text)
        {
            if (!Regex.IsMatch(text, "^[0-9]+$"))
                return false;
            return true;
        }

        public static bool onlyLetters(string text)
        {
            if (!Regex.IsMatch(text, @"^[a-zA-Z ]+$"))
                return false;
            return true;
        }

        public static void ExtractFileZip(string pathFile, string extractFolder)
        {
            bool isExists = Directory.Exists(extractFolder);
            if (!isExists)
                Directory.CreateDirectory(extractFolder);
            using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(pathFile))
            {
                zip.ExtractAll(extractFolder, ExtractExistingFileAction.OverwriteSilently);
            }
        }


        public static string CreateZipFile(string pathToSave , List<string> l)
        {
            var zipFile = pathToSave;
            using (var archive = System.IO.Compression.ZipFile.Open(zipFile, ZipArchiveMode.Create))
            {
                foreach (var fPath in l)
                {
                    archive.CreateEntryFromFile(fPath, fPath.Split('\\').Last());
                }
            }
            return zipFile;
        }

        public static void SendViaFtp(string localFilePath, string uploadFileName, string ftpUsername = usernameFtp, string ftpPassword = passwordFtp, string pathFtp = ftpUrl + ftpInputFolder)
        {
            Uri uri = new Uri("ftp://ftp.ewt.group/httpdocs/special/Input/" + uploadFileName);
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
            client.UploadFile(uri, localFilePath);
        }



        public static byte[] getByteFromPdf(string url)
        {
            byte[] b = null;
            try
            {
             b = File.ReadAllBytes(url);
            }
                catch (Exception e)
            {
                var t = e;
            }
            return b;
        }

        public static decimal GetVatFromNetPrice(decimal netPrice)
        {
            return netPrice * (decimal)vat;
        }

        public static Comune GetComuneFromCapOld(string cap)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            cap = cap.Replace(" ", "");
            var comune = comuni.Where(x => x.cap.ToString().Contains(cap));
            int i = comune.Count();

            if (i == 0)
                return null;

            var comuniList = comune.ToList();

            var c = new Comune()
            {
                cap = cap,
                comune = comuniList[0].nome,
                provincia = comuniList[0].provincia.nome,
                sigla = comuniList[0].sigla
            };

            return c;
        }

        public static async Task<bool> sendEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage MailMessage = new MailMessage();
                MailMessage.From = "noreply@easysender.it";
                MailMessage.To = to;
                MailMessage.Subject = subject;
                MailMessage.BodyFormat = MailFormat.Html;
                MailMessage.Body = "<html>" +
                "<body style='background:#f5f5f5; padding:50px 0 200px 0; width:100%;'>" +
                        "<table cellpadding=0 cellspacing=0 width=620 style='padding:10px; font-size:12px; font-family:arial; background:#fff; border-radius: 8px; box-shadow: 0 1px 10px 0 rgba(0, 0, 0, 0.1); padding: 30px 35px;'; align='center'>" +
                            "<tr height='60'><td valign='absmiddle' align='center' style='text-align:left; padding:0 0 15px 0'><img src='http://app.easysender.it/Images/logo-orizzontale.png' alt='' width='220'></td></tr>" +
                            "<tr>" +
                                "<td style='line-height:18px; padding:20px 10px; color:#666; text-align:left; border-top:solid 1px #f1f1f1; border-bottom:solid 1px #f1f1f1;'>" +
                                    body +
                                "</td>" +
                            "</tr>" +
                        "</table>" +
                    "</html>" +
                    "</body>";


                MailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", 1);
                MailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", "noreply@easysender.it");
                MailMessage.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", "PwdReply20");

                SmtpMail.SmtpServer = "mail.easysender.it";
                SmtpMail.Send(MailMessage);

                return true;


            }
            catch (Exception e)
            {
                var x = e.InnerException;
            }
            return false;
        }

        public static List<Comune> GetComuneFromCap(string cap)
        {
            var comuni = GetComuniList();
            var comune = comuni.Where(a => a.cap == cap).ToList();
            int x = comune.Count();

            if (x == 0)
                return null;

            var l = new List<Comune>();
            foreach(var cc in comune) { 
                var c = new Comune()
                {
                    cap = cap,
                    comune = cc.comune,
                    provincia = null,
                    sigla = cc.sigla
                };
                l.Add(c);
            }

            return l;

        }

        public static List<ComuniXLS> GetComuniList()
        {
            var package = new ExcelPackage(new FileInfo(HttpContext.Current.Server.MapPath("/json/cap.xlsx")));

            ExcelWorksheet workSheet = package.Workbook.Worksheets[2];

            var comuni = new List<ComuniXLS>();

            for (int i = workSheet.Dimension.Start.Row; i <= workSheet.Dimension.End.Row; i++)
            {
                if (i > 1)
                {
                    var comuneXLS = new ComuniXLS()
                    {
                        cap = workSheet.Cells[i, 1].Value.ToString(),
                        comune = workSheet.Cells[i, 2].Value.ToString(),
                        sigla = workSheet.Cells[i, 3].Value.ToString()
                    };
                    comuni.Add(comuneXLS);

                }
            }

            return comuni;
        }
        
        public static List<string> GetNazioniList()
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/upu.json"));
            r = r.ToLower();
            r = r.Replace("citt� del vaticano", "città del vaticano");
            r = r.Replace("indon�sie", "indonésie");
            r = r.Replace("per�", "perù");
            r = r.ToUpper();
            var n = JsonConvert.DeserializeObject<List<string>>(r);
            return n;
        }

        public static MemoryStream SerializeToStream(object objectType)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, objectType);
            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object objectType = formatter.Deserialize(stream);
            return objectType;
        }

        //RANDOMIZE CODICE CLIENTE BOLLETTINI
        public static ComuniItaliani GetComuneItalianoFromCap(string cap)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            cap = cap.Replace(" ", "");
            var comune = comuni.Where(x => x.cap.ToString().Contains(cap)).ToList();

            if (comune.Count() == 0)
                return null;

            var c = new ComuniItaliani()
            {
                cap = cap,
                provincia = null,
                sigla = comune[0].sigla,
                codiceCatastale = comune[0].codiceCatastale,
                nome = comune[0].nome
            };

            return c;

        }

        public static string GetCodiceCatastale(string letters)
        {
            var letter = letters[0].ToString();
            string code = "01";
            switch (letter.ToUpper())
            {
                case "A":
                    code = "01";
                    break;
                case "B":
                    code = "02";
                    break;
                case "C":
                    code = "03";
                    break;
                case "D":
                    code = "04";
                    break;
                case "E":
                    code = "05";
                    break;
                case "F":
                    code = "06";
                    break;
                case "G":
                    code = "07";
                    break;
                case "H":
                    code = "08";
                    break;
                case "I":
                    code = "09";
                    break;
                case "L":
                    code = "10";
                    break;
                case "M":
                    code = "11";
                    break;

                default:
                    code = "01";
                    break;
            }

            return code;
        }

        public static string GetCodiceCatastaleNumeri(string letters)
        {
            var n1 = letters[1].ToString();
            var n2 = letters[2].ToString();
            var n3 = letters[3].ToString();
            string code = n1 + n2 + n3;
            return code;
        }

        private static Random random = new Random();
        public static string RandomCode(int length)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GetCodiceClienteBollettino(string anno, string cap)
        {
            var comune = GetComuneItalianoFromCap(cap);
            if (comune == null)
                return "";

            int divisore = 93;
            var y = anno[3].ToString();
            var code = GetCodiceCatastale(comune.codiceCatastale);
            var numbersCode = GetCodiceCatastaleNumeri(comune.codiceCatastale);
            String c = "";

            do
            {
                c = "";
                var randomCode = RandomCode(10);

                c = y + code + numbersCode + randomCode;

                long cc = Convert.ToInt64(c);

                var d = cc % divisore;
                var sd = Convert.ToString(d);

                c += sd;

            }while (c.Length != 18);
            
            return c;
        }

        //CREAZIONE PDF DA DOC
        //MERGE FIELD
        public static List<string> ReadCsvCreatePDF(string pathCsv, string pathDoc, string filePath)
        {
            //CREAZIONE DIRECTORY 
            //PER IL SALVATAGGIO DEI PDF
            var pathToSave = filePath + "/Pdf";
            bool isExistsS = Directory.Exists(pathToSave);
            if (!isExistsS)
                Directory.CreateDirectory(pathToSave);


            var r = new List<string>();
            try
            {
                string fileList = Globals.GetCSVFromRemote(pathCsv);
                string[] tempStr;
                tempStr = fileList.Split((char)10);
                int i = 0;

                var comuni = Globals.GetComuniList();

                int positionBusinessName = 0;
                int positionName = 0;
                int positionSurname = 0;
                int positionAddress = 0;
                int positionCap = 0;
                int positionCity = 0;
                int positionProvince = 0;
                int positionState = 0;
                int positionFileName = 0;
                int positionComplementAddress = 0;
                int positionComplementNames = 0;
                int positionCf = 0;
                int positionMobile = 0;
                ArrayList attachedFieldName = null;
                ArrayList attachedFieldValue = null;

                foreach (string row in tempStr)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(row))
                        {
                            string[] column = row.Replace(Convert.ToString((char)13), "").Split(';');
                            var cl = column.Length;
                            if (column.Length < 12)
                            {
                                return null;
                            }


                            if (i == 0)
                            {
                                for (var x = 0; x < column.Length; x++)
                                {
                                    switch (column[x].ToUpper())
                                    {
                                        case "RAGIONESOCIALE":
                                        case "RAGIONE SOCIALE":
                                            positionBusinessName = x;
                                            break;
                                        case "NOME":
                                            positionName = x;
                                            break;
                                        case "COGNOME":
                                            positionSurname = x;
                                            break;
                                        case "INDIRIZZO":
                                            positionAddress = x;
                                            break;
                                        case "CAP":
                                            positionCap = x;
                                            break;
                                        case "CITTA":
                                            positionCity = x;
                                            break;
                                        case "PROVINCIA":
                                            positionProvince = x;
                                            break;
                                        case "STATO":
                                            positionState = x;
                                            break;
                                        case "NOMEFILE":
                                        case "NOME FILE":
                                            positionFileName = x;
                                            break;
                                        case "COMPLETAMENTO INDIRIZZO":
                                        case "COMPLETAMENTOINDIRIZZO":
                                            positionComplementAddress = x;
                                            break;
                                        case "COMPLETAMENTO NOMINATIVO":
                                        case "COMPLETAMENTONOMINATIVO":
                                            positionComplementNames = x;
                                            break;
                                        case "CODICE FISCALE":
                                        case "CODICEFISCALE":
                                        case "CF":
                                            positionCf = x;
                                            break;
                                        case "TELEFONO":
                                        case "MOBILE":
                                        case "CELLULARE":
                                        case "CELL":
                                            positionMobile = x;
                                            break;
                                    }
                                };

                                if (column.Length > 13) {
                                    attachedFieldName = new ArrayList();
                                    for (var a = 13; a < column.Length; a++)
                                        attachedFieldName.Add(column[a]);
                                }
                            }
                            else
                            {
                                var comune = comuni.Where(c => c.cap == column[positionCap]).ToList();

                                Names n = new Names();
                                n.name = column[positionName];
                                n.surname = column[positionSurname];
                                n.businessName = column[positionBusinessName];
                                n.address = column[positionAddress];
                                n.cap = column[positionCap];
                                n.city = column[positionCity];
                                n.province = column[positionProvince];
                                n.state = column[positionState];
                                n.fileName = column[positionFileName];
                                n.complementAddress = column[positionComplementAddress];
                                n.complementNames = column[positionComplementNames];
                                n.fiscalCode = column[positionCf];
                                n.mobile = column[positionMobile];

                                //CAMPI AGGIUNTIVI 
                                if (column.Length > 13)
                                {
                                    attachedFieldValue = new ArrayList();
                                    for (var b = 13; b < column.Length; b++)
                                        attachedFieldValue.Add(column[b]);
                                }

                                r.Add(GetPdfFromDoc(pathToSave, pathDoc, n, attachedFieldName, attachedFieldValue));
                            }
                            i++;
                        }
                    }
                    catch (Exception e)
                    {
                        var ee = e;
                    }
                }
            }
            catch (Exception e)
            {
                var t = e;
            }

            return r;
        }

        public static string GetPdfType(string path)
        {
            try 
            { 
                GemBox.Document.ComponentInfo.SetLicense("DN-2018Dec12-9sddmSffNixTdA+u3QCsByLiDXWSBEtYKABxkYzM3bveyu8kGY+T9jaKK1inDryXoeiUGM8g4CBp4Iu0864RzLA37hQ==A");

                var document = DocumentModel.Load(path);

                var t = document.Sections[0].PageSetup.PaperType;

                return t.ToString();
            }
            catch(Exception e)
            {
                return "A4";
            }
        }

        public static string GetPdfFromDoc(string pathToSave, string pathDoc, Names n, ArrayList attacchedFieldNames = null, ArrayList attacchedFieldValues = null)
        {
            GemBox.Document.ComponentInfo.SetLicense("DN-2018Dec12-9sddmSffNixTdA+u3QCsByLiDXWSBEtYKABxkYzM3bveyu8kGY+T9jaKK1inDryXoeiUGM8g4CBp4Iu0864RzLA37hQ==A");

            var document = DocumentModel.Load(pathDoc);

            IDictionary<string, object> d = new ExpandoObject();
            d["Nome"] = n.name;
            d["Cognome"] = n.surname;
            d["RagioneSociale"] = n.businessName;
            d["Indirizzo"] = n.address;
            d["CAP"] = n.cap;
            d["Provincia"] = n.province;
            d["Citta"] = n.city;

            if (attacchedFieldNames != null) 
                for(var x = 0; x < attacchedFieldNames.Count; x++)
                    d[attacchedFieldNames[x].ToString()] = attacchedFieldValues[x];


            document.MailMerge.Execute(d);

            var fileName = n.fileName;
            if (fileName == null)
                fileName = DateTime.Now.Ticks.ToString() + ".pdf";

            var fileSaved = fileName;

            document.Save(pathToSave + "/" + fileName);

            return fileSaved;
        }

        public static string GetTextFromRectangle(string pathPdf, int a, int b, int c, int d)
        {
            var txt = "";
            //329,695
            // If using Professional version, put your serial key below.
            //GemBox.Pdf.ComponentInfo.SetLicense("ADWG-YKI0-D7LE-5JK9");

            var pageIndex = 0;
            var area = new Rectangle(a, b, c, d);

            //using (var document = PdfDocument.Load(pathPdf))
            //{
            //    // Retrieve first page object.
            //    var page = document.Pages[pageIndex];

            //    // Retrieve text content elements that are inside specified area on the first page.
            //    foreach (var textElement in page.Content.Elements.All()
            //        .Where(element => element.ElementType == PdfContentElementType.Text)
            //        .Cast<PdfTextContent>())
            //    {
            //        var location = textElement.Location;
            //        if (location.X > area.X && location.X < area.X + area.Width &&
            //            location.Y > area.Y && location.Y < area.Y + area.Height)
            //            txt += textElement.ToString();
            //    }
            //}
            //return txt;

            var Ocr = new AdvancedOcr() {
                Language = IronOcr.Languages.Italian.OcrLanguagePack
            };
            var Result = Ocr.Read(pathPdf, area);
            var r = Result.Text;
            return r;
        }
    }
}