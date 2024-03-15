using Api.DataModel;
using Api.Dtos;
using Api.Models;
using Api.ServiceLol;
using AutoMapper;
using GemBox.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api.Controllers.Api
{
    /// <summary>
    /// Raccomandata online
    /// </summary>
    [RoutePrefix("api/Lol")]
    public class LolController : ApiController
    {

        private static Entities _context;

        public LolController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("ServiceLol")]
        [HttpGet]
        public static LOLServiceSoapClient getNewServiceLol(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;

            LOLServiceSoapClient service = new LOLServiceSoapClient();
            if (Users.areaTestUser)
            {
                service.ClientCredentials.UserName.UserName = Users.usernamePosteAreaTest;
                service.ClientCredentials.UserName.Password = Users.pwdPosteAreaTest;
            }
            else
            {
                service.ClientCredentials.UserName.UserName = Users.usernamePoste;
                service.ClientCredentials.UserName.Password = Users.pwdPoste;
            }
            return service;
        }

        private static string getRequestId(Guid guid)
        {
            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guid);
            var IdRichiesta = service.RecuperaIdRichiesta().IDRichiesta;
            service.Close();
            return IdRichiesta;
        }

        private static string getStaticRequestId()
        {
            var IdRichiesta = Guid.NewGuid();
            return IdRichiesta.ToString();
        }

        private static Destinatario GetDestinatarioLol(NamesDto name, int? index = 1)
        {
            Destinatario d = new Destinatario();
            Nominativo n = new Nominativo();
            Indirizzo i = new Indirizzo();
            n.RagioneSociale = name.businessName;
            n.Cognome = name.surname;
            n.Nome = name.name;
            n.ComplementoNominativo = name.complementNames;
            n.CAP = name.cap;
            n.Citta = name.city;
            n.Provincia = name.province;
            n.Stato = name.state;
            n.ComplementoIndirizzo = name.complementAddress;
            n.CodiceFiscale = name.fiscalCode;

            i.DUG = name.dug;
            i.Toponimo = name.address;
            i.NumeroCivico = name.houseNumber;
            i.Esponente = string.Empty;
            n.Indirizzo = i;

            n.Telefono = string.Empty;
            n.TipoIndirizzo = NominativoTipoIndirizzo.NORMALE;
            n.UfficioPostale = "";
            n.Zona = string.Empty;
            n.CasellaPostale = "";
            n.Frazione = string.Empty;

            d.IdDestinatario = Convert.ToString(index);
            d.IdRicevuta = String.Empty;
            d.Nominativo = n;

            return d;
        }

        private static OpzionidiStampa GetOpzioniDiStampa(tipoStampa tipoStampa, fronteRetro fronteRetro)
        {
            var opzioniDiStampa = new OpzionidiStampa();
            opzioniDiStampa.BW = (tipoStampa == tipoStampa.biancoNero ? "true" : "false");
            opzioniDiStampa.FronteRetro = (fronteRetro == fronteRetro.fronteRetro ? "true" : "false");
            return opzioniDiStampa;
        }

        private static LOLSubmitOpzioni GetOpzioniLol(tipoStampa tipoStampa, fronteRetro fronteRetro)
        {

            var Opzioni = new LOLSubmitOpzioni();
            Opzioni.Archiviazione = false;
            Opzioni.DataStampa = DateTime.Now;
            Opzioni.DPM = false;
            Opzioni.OpzionidiStampa = GetOpzioniDiStampa(tipoStampa, fronteRetro);
            Opzioni.FirmaElettronica = false;
            Opzioni.InserisciMittente = false;
            Opzioni.Inserti = new LOLSubmitOpzioniInserti();
            Opzioni.Inserti.InserisciMittente = false;
            Opzioni.Inserti.Inserto = string.Empty;

            return Opzioni;
        }

        private static Mittente GetMittente(SenderDto sender)
        {

            Mittente Mittente = new Mittente();
            Nominativo Nominativo = new Nominativo();
            Indirizzo Indirizzo = new Indirizzo();

            Nominativo.CAP = sender.cap;
            Nominativo.Citta = sender.city;
            Nominativo.Cognome = sender.surname;
            Nominativo.Nome = sender.name;
            Nominativo.Provincia = sender.province;
            Nominativo.Stato = sender.state;
            Nominativo.RagioneSociale = sender.businessName;

            Indirizzo.DUG = sender.dug;
            Indirizzo.NumeroCivico = sender.houseNumber;
            Indirizzo.Toponimo = sender.address;
            Indirizzo.Esponente = string.Empty;

            Nominativo.Telefono = string.Empty;
            Nominativo.TipoIndirizzo = NominativoTipoIndirizzo.NORMALE;
            Nominativo.UfficioPostale = "";
            Nominativo.Zona = string.Empty;
            Nominativo.ComplementoIndirizzo = sender.complementAddress;
            Nominativo.CasellaPostale = "";
            Nominativo.ComplementoNominativo = sender.complementNames;
            Nominativo.Frazione = string.Empty;
            Nominativo.Indirizzo = Indirizzo;

            Mittente.Nominativo = Nominativo;
            Mittente.InviaStampa = false;

            return Mittente;
        }

        public static Documento[] getDoc(List<string> strNomeFile, int NumeroDiDocumenti)
        {
            System.IO.FileInfo file;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Documento[] ArrayDocumento = new Documento[NumeroDiDocumenti - 1 + 1];
            for (var i = 0; i <= NumeroDiDocumenti - 1; i++)
            {
                Documento documento = new Documento();
                file = new System.IO.FileInfo(strNomeFile[i]);
                documento.TipoDocumento = "pdf";
                documento.Immagine = System.IO.File.ReadAllBytes(strNomeFile[i]);
                documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(documento.Immagine)).Replace("-", string.Empty);
                ArrayDocumento[i] = documento;
            }
            return ArrayDocumento;
        }

        public static Bollettino896 getBollettino896(BulletinsDtos bollettino)
        {
            Bollettino896 b = new Bollettino896();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.FormatoStampa = 0;
            b.AdditionalInfo = "";
            b.IBAN = bollettino.IBAN;
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.CodiceCliente = bollettino.codiceCliente;
            b.ImportoEuro = bollettino.importoEuro;
            b.Causale = bollettino.causale;
            return b;
        }
        public List<GetSubmitResponses> publicGetSubmitResponse = new List<GetSubmitResponses>();
        public List<GetSubmitResponses> publicGetValorizzaResponse = new List<GetSubmitResponses>();
        public List<GetSubmitResponse> publicGetSubmitResponseConfirm = new List<GetSubmitResponse>();

        private void createFeatures(tipoStampa tipoStampa, fronteRetro fronteRetro, int operationId)
        {
            var op1 = new operationFeatures();
            op1.operationId = operationId;
            op1.featureType = "tipo stampa";
            op1.featureValue = Enum.GetName(typeof(tipoStampa), tipoStampa);
            _context.operationFeatures.Add(op1);
            _context.SaveChanges();

            var op2 = new operationFeatures();
            op2.operationId = operationId;
            op2.featureType = "fronte Retro";
            op2.featureValue = Enum.GetName(typeof(fronteRetro), fronteRetro);
            _context.operationFeatures.Add(op2);
            _context.SaveChanges();

        }


        [Route("NewOne")]
        [HttpPost]
        public async Task<int> InsertOne([FromUri] int operationId, [FromUri] string tsc,
            [FromUri] string frc, [FromUri] string frm, [FromUri] int userId, [FromBody] NamesDto n, string TipoLettera = "")
        {

            //UTENTE INSERITORE
            var u = _context.Users.SingleOrDefault(a => a.id == userId);

            //INSERISCI SENDER
            //var sender = _context.SendersUsers.FirstOrDefault(a => a.id == senderId);
            //SenderDtos ss = Mapper.Map<SendersUsers, SenderDtos>(sender);
            //ss.operationId = operationId;
            //int sId = SenderController.CreateItem(ss);

            //INSERISCI NAMES

            NamesDtos nos = Mapper.Map<NamesDto, NamesDtos>(n);
            nos.operationId = operationId;
            nos.requestId = null;
            nos.guidUser = null;
            nos.valid = true;

            bool fronteRetro = true;
            if (frc == "0")
                fronteRetro = false;

            bool tipoStampa = true;
            if (tsc == "0")
                tipoStampa = false;

            nos.fronteRetro = fronteRetro;
            nos.ricevutaRitorno = false;
            nos.tipoStampa = tipoStampa;
            nos.tipoLettera = TipoLettera;

            nos.insertDate = DateTime.Now;
            nos.currentState = (int)currentState.inAttesa;

            //NUMERO DI PAGINE
            //ComponentInfo.SetLicense("ADWG-YKI0-D7LE-5JK9");
            //var document = PdfDocument.Load(nos.fileName);
            //nos.numberOfPages = document.Pages.Count();

            var nc = new NamesController();
            int idName = nc.CreateItem(nos, u.userPriority);


            return idName;
        }

        [Route("CheckAllFiles")]
        [HttpPost]
        public async Task<GetNumberOfCheckedNames> CheckAllFiles([FromUri] Guid guidUser, [FromBody] ObjectSubmit senderRecipients, [FromUri] bool tsc,
            [FromUri] bool frc, [FromUri] bool frm, [FromUri] int userId, [FromUri] string TipoLettera = "")
        {
            SenderDto sender = new SenderDto();
            sender = senderRecipients.sender;
            List<GetRecipent> GetRecipents = senderRecipients.recipients;

            GetNumberOfCheckedNames ncn = new GetNumberOfCheckedNames();
            List<GetCheckedNames> lgcn = new List<GetCheckedNames>();

            //MULTIPLE USERS
            var users = _context.Users.Where(a => a.guidUser == guidUser);

            //ERRORE GUID
            if (users.Count() == 0)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Utente non riconosiuto";
                return ncn;
            }

            //UTENTE INSERITORE
            var u = new Users();
            if (userId > 0)
                u = users.SingleOrDefault(a => a.id == userId);
            else
                u = users.SingleOrDefault(a => a.parentId == 0);


            //ERRORE MITTENTE
            ControlloMittente ctrlM = GlobalClass.verificaMittente(senderRecipients.sender);
            if (!ctrlM.Valido)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Mittente non valido";
                return ncn;
            }

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.LOL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = false;
            op.formatoSpeciale = frm;
            op.csvFileName = senderRecipients.csvFile;
            int operationId = OperationsController.CreateItem(op);

            tipoStampa ts = tipoStampa.colori;
            if (tsc)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (frc)
                fr = fronteRetro.fronteRetro;

            createFeatures(ts, fr, operationId);

            SenderDtos ss = Mapper.Map<SenderDto, SenderDtos>(sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            int validNames = 0;
            foreach (var GetRecipent in GetRecipents.ToList())
            {
                int id = (int)GetRecipent.recipient.id;
                var b = _context.Bulletins.Where(a => a.namesListsId == id).ToList();
                if (b.Count() > 0)
                {
                    GetRecipent.bulletin = Mapper.Map<Bulletins, BulletinsDtos>(b[0]);
                };

                NamesDtos nos = Mapper.Map<NamesDto, NamesDtos>(GetRecipent.recipient);
                nos.operationType = op.operationType;
                nos.operationId = operationId;
                nos.requestId = null;
                nos.guidUser = null;
                nos.valid = true;

                nos.fronteRetro = frc;
                nos.ricevutaRitorno = false;
                nos.tipoStampa = tsc;
                nos.tipoLettera = TipoLettera;

                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

                //NUMERO DI PAGINE
                PdfDocument document = new PdfDocument();
                try 
                { 
                    document = PdfDocument.Load(nos.fileName);
                    ComponentInfo.SetLicense("ADWG-YKI0-D7LE-5JK9");
                    nos.numberOfPages = document.Pages.Count();
                }
                catch(Exception e) { }

                var nc = new NamesController();
                int idName = nc.CreateItem(nos, u.userPriority);
                if (GetRecipent.bulletin != null)
                {
                    BulletinsDto bos = Mapper.Map<BulletinsDtos, BulletinsDto>(GetRecipent.bulletin);
                    bos.namesId = idName;
                    BulletinsController.CreateItem(bos);
                }
                validNames++;

                GetCheckedNames gcn = new GetCheckedNames()
                {
                    name = nos,
                    valid = true,
                    price = frm ? GlobalClass.GetFilePriceSpecialFormat(operationType.LOL, document, tsc) : GlobalClass.GetFilePrice(operationType.LOL, document, tsc)
                };

                lgcn.Add(gcn);

            }

            ncn.numberOfValidNames = validNames;
            ncn.checkedNames = lgcn;
            ncn.state = "Inserimento valido!";
            ncn.valid = true;
            ncn.operationId = operationId;

            return ncn;

        }

        [Route("RequestDCS")]
        [HttpGet]
        public string RequestDCS(Guid guidUser, int id)
        {
            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            Richiesta IdRol = new Richiesta();
            IdRol.GuidUtente = name.guidUser;
            IdRol.IDRichiesta = name.requestId;

            var dcs = service.RecuperaDCS(IdRol);
            var nameFile = "";

            if (dcs.CEResult.Type == "I")
            {
                var n = DateTime.Now.Ticks + ".pdf";
                nameFile = "/public/download/" + n;
                var path = HttpContext.Current.Server.MapPath(nameFile);
                System.IO.File.WriteAllBytes(path, dcs.Documento.Immagine);

                //SALVATAGGIO TEMPORANEO FILE
                //FINO A SETT/OTT 2020
                name.pathRecoveryFile = nameFile;
                _context.SaveChanges();

                GlobalClass.SendViaFtp(path, n, GlobalClass.usernameFtpDoc, GlobalClass.passwordFtpDoc, GlobalClass.ftpUrlDoc);
                File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("/public/download/"), n));

            }

            return nameFile;
        }


    }
}
