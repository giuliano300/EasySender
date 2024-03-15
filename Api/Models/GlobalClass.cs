using Api.Dtos;
using AutoMapper;
using GemBox.Pdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Api.Models
{
    public class GlobalClass
    {
        public static string siteName = "EWT";

        public static string baseUrl = "https://publicapi.easysender.it/";
        public static string DescrizioneTipoPagamento = "Post fatturato al cliente";
        public static string IdTipoPagamento = "6";
        public static int vat = 22;
        public static string downladFile = "https://private.easysender.it/private-files";

        public static int pageNumber(int page=0) {
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

        public static string leftString(string Stringa,int index)
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
            if(ragioneSociale != null)
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
                // CONTROLLO NUMERO CIVICO
                if (numeroCivico.Length > 5)
                {
                    ctrl.Errore = "il numero civico supera i 5 caratteri";
                    ctrl.Valido = false;
                }

                if (dug.Length <= 0)
                {
                    ctrl.Errore = "il DUG non è valido";
                    ctrl.Valido = false;
                }
                if (dug.Length > 10)
                {
                    ctrl.Errore = "il DUG supera i 10 caratteri";
                    ctrl.Valido = false;
                }
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
            if(codiceControllo != lastCod)
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

        public static ControlloDestinatarioInsert verificaDestinatarioInsert(NameInsertDto recipient, string pathUrl = null)
        {
            var file = HttpRuntime.AppDomainAppPath + "/json/comuniItaliani.json";
            var r = File.ReadAllText(file);
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();
            ListaControlloDestinatari l = new ListaControlloDestinatari();

            NameInsertDto Destinatario = recipient;
            ControlloDestinatarioInsert ctrl = new ControlloDestinatarioInsert();
            ctrl.Destinatario = Destinatario;

            // CONTROLLO CAP SOLO SE ITALIA
            if (recipient.state.ToUpper() == "ITALIA") { 
                string Cap = Destinatario.cap.Replace(" ", "");
                var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
                int i = comune.Count();

                crt crt = verificaCap(Cap, i);
                if (!crt.Valido)
                {
                    ctrl.Valido = crt.Valido;
                    ctrl.Errore = crt.Errore + " - Destinatario";
                }
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
            crt crtCC = GlobalClass.verificaCodiceCliente(b.codiceCliente);
            if (!crtCC.Valido)
            {
                ctrl.Valido = false;
                ctrl.Errore = crtCC.Errore;
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
            if (b.bulletinType != (int)bulletinType.Bollettino451 && b.bulletinType != (int)bulletinType.Bollettino674 && b.bulletinType != (int)bulletinType.Bollettino896)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo BulletinType errato - Bollettino";
            }


            return ctrl;
        }

        public static ControlloBollettinoInsert verificaBollettinoInsert(BulletinInsertDto b)
        {
            ControlloBollettinoInsert ctrl = new ControlloBollettinoInsert();
            ctrl.Bollettino = b;

            // CONTROLLO CONTO CORRENTE
            if (b.NumeroContoCorrente == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente non valido - Bollettino";
            }

            if (!onlyNumbers(b.NumeroContoCorrente))
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente deve contenere solo numeri";
            }

            if (b.NumeroContoCorrente.Length != 12)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente non ha la lunghezza consentita di 12 numeri";
            }

            // Intestato A
            if (b.IntestatoA == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo IntestatoA vuoto - Bollettino";
            }

            // CodiceCliente
            crt crtCC = GlobalClass.verificaCodiceCliente(b.CodiceCliente);
            if (!crtCC.Valido)
            {
                ctrl.Valido = false;
                ctrl.Errore = crtCC.Errore;
            }

            // ImportoEuro
            if (b.ImportoEuro == 0)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo ImportoEuro vuoto - Bollettino";
            }

            // EseguitoDaNominativo
            if (b.EseguitoDaNominativo == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaNominativo vuoto - Bollettino";
            }

            // EseguitoDaIndirizzo
            if (b.EseguitoDaIndirizzo == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaIndirizzo vuoto - Bollettino";
            }

            // EseguitoDaIndirizzo
            crt crt = verificaCap(b.EseguitoDaCAP, 1);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Bollettino";
            }

            // EseguitoDaLocalita
            if (b.EseguitoDaLocalita == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaLocalita vuoto - Bollettino";
            }

            // Causale
            if (b.Causale == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo Causale vuoto - Bollettino";
            }

            // BulletinType
            if (b.BulletinType != (int)bulletinType.Bollettino451 && b.BulletinType != (int)bulletinType.Bollettino674 && b.BulletinType != (int)bulletinType.Bollettino896)
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

        public static ControlloMittenteInsert verificaMittenteInsert(SenderInsertDto sender)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);

            SenderInsertDto s = sender;
            ControlloMittenteInsert ctrl = new ControlloMittenteInsert();
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
            array[0] = 0 ;
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

        public static Prices GetFilePrice(operationType type, string fileName, bool colori)
        {
            decimal totale = 0;
            ComponentInfo.SetLicense("ADWG-YKI0-D7LE-5JK9");
            var document = PdfDocument.Load(fileName);

            if (document.Pages.Count == 1)
            {
                if (type == operationType.LOL)
                    if(colori)
                        totale = Convert.ToDecimal(2.49);
                    else
                        totale = Convert.ToDecimal(2.32);

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.00);
                    else
                        totale = Convert.ToDecimal(3.70);
            }

            if (document.Pages.Count == 2)
            {
                if (type == operationType.LOL)
                    if (colori)
                        totale = Convert.ToDecimal(2.60);
                    else
                        totale = Convert.ToDecimal(2.40);

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.15);
                    else
                        totale = Convert.ToDecimal(3.80);
            }

            if (document.Pages.Count == 3)
            {
                if (type == operationType.LOL)
                    if (colori)
                        totale = Convert.ToDecimal(2.69);
                    else
                        totale = Convert.ToDecimal(2.46);

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(4.30);
                    else
                        totale = Convert.ToDecimal(3.90);
            }

            if (document.Pages.Count == 4 || document.Pages.Count == 5)
            {
                if (type == operationType.LOL)
                    if (colori)
                        totale = Convert.ToDecimal(2.81);
                    else
                        totale = Convert.ToDecimal(2.55);

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.35);
                    else
                        totale = Convert.ToDecimal(5.05);
            }

            if (document.Pages.Count == 6 || document.Pages.Count == 7 || document.Pages.Count == 8 || document.Pages.Count == 9)
            {
                if (type == operationType.LOL)
                    if (colori)
                        totale = Convert.ToDecimal(3.03);
                    else
                        totale = Convert.ToDecimal(2.67);

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.70);
                    else
                        totale = Convert.ToDecimal(5.10);
            }

            if (document.Pages.Count == 10 || document.Pages.Count == 11 || document.Pages.Count == 12 || document.Pages.Count == 13)
            {
                if (type == operationType.LOL)
                    if (colori)
                        totale = Convert.ToDecimal(3.33);
                    else
                        totale = Convert.ToDecimal(2.85);

                if (type == operationType.ROL)
                    if (colori)
                        totale = Convert.ToDecimal(5.85);
                    else
                        totale = Convert.ToDecimal(5.15);
            }

            if (document.Pages.Count>13)
            {
                if (type == operationType.LOL)
                    if (colori)
                        totale = Convert.ToDecimal(3.78);
                    else
                        totale = Convert.ToDecimal(3.15);

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