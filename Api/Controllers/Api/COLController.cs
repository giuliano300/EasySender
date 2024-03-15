using Api.DataModel;
using Api.Dtos;
using Api.Models;
using Api.ServiceCOL;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/COL")]
    public class COLController : ApiController
    {
        private static Entities _context;

        public COLController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        private static Opzioni GetOpzioni(tipoStampa tipoStampa, fronteRetro fronteRetro)
        {
            var opzioni = new Opzioni();

            var opzioniStampa = new OpzioniStampa();
            opzioniStampa.TipoColore = (tipoStampa == tipoStampa.colori ? TipoColore.COLORE : TipoColore.BW);
            opzioniStampa.FronteRetro = (fronteRetro == fronteRetro.fronteRetro ? true : false);

            var servizio = new OpzioniServizio();
            servizio.ArchiviazioneDocumenti = false;

            opzioni.Stampa = opzioniStampa;
            opzioni.Servizio = servizio;

            return opzioni;
        }

        private static PostaContestServiceClient getNewServiceCOL(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;
            PostaContestServiceClient service = new PostaContestServiceClient();
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

        private static Mittente GetMittente(SenderDto sender, string businessName)
        {
            Mittente m = new Mittente();

            m.Nominativo = businessName;
            m.ComplementoIndirizzo = "";
            m.ComplementoNominativo = "";
            m.Indirizzo = sender.dug + " " + sender.address + " " + sender.houseNumber;
            m.Cap = sender.cap;
            m.Comune = sender.city;
            m.Provincia = sender.province;
            m.Nazione = sender.state;

            return m;
        }

        public static Bollettino896 getBollettino896(BulletinsDtos bollettino)
        {
            Bollettino896 b = new Bollettino896();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.CodiceCliente = bollettino.codiceCliente;
            b.ImportoEuro = bollettino.importoEuro;
            b.Causale = bollettino.causale;
            if (bollettino.IBAN != null && bollettino.IBAN != "")
                b.IBAN = bollettino.IBAN;
            b.Template = "896";
            return b;
        }

        public static Documento[] getDoc(List<string> strNomeFile, int NumeroDiDocumenti)
        {
            System.IO.FileInfo file;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Documento[] ArrayDocumento = new Documento[NumeroDiDocumenti - 1 + 1];
            for (var i = 0; i <= NumeroDiDocumenti - 1; i++)
            {
                Documento documento = new Documento();
                var immagine = System.IO.File.ReadAllBytes(strNomeFile[i]);
                documento.Estensione = "pdf";
                documento.Contenuto = immagine;
                documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(immagine)).Replace("-", string.Empty);
                ArrayDocumento[i] = documento;
            }
            return ArrayDocumento;
        }

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

        [Route("CheckAllFiles")]
        [HttpPost]
        public async Task<GetNumberOfCheckedNames> CheckAllFiles([FromUri] Guid guidUser, [FromBody] ObjectSubmit senderRecipients, [FromUri] bool tsc,
            [FromUri] bool frc, [FromUri] int userId)
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
            op.operationType = (int)operationType.COL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = false;
            op.csvFileName = senderRecipients.csvFile;

            int operationId = OperationsController.CreateItem(op);

            tipoStampa ts = tipoStampa.colori;
            if (tsc)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (!frc)
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
                nos.operationId = operationId;
                nos.operationType = op.operationType;
                if (!nos.state.ToUpper().Contains("ITALIA") && !nos.state.ToUpper().Contains("ITA"))
                    nos.operationType = (int)operationType.LOL;
                nos.requestId = null;
                nos.guidUser = null;
                nos.valid = true;

                nos.fronteRetro = Convert.ToBoolean(fr);
                nos.ricevutaRitorno = false;
                nos.tipoStampa = Convert.ToBoolean(ts);

                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

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
                    price = new Prices()
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
            var user = _context.Users.Where(a => a.guidUser == guidUser).FirstOrDefault();

            PostaContestServiceClient service = new PostaContestServiceClient();
            service = getNewServiceCOL(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            RecuperaDocumentoRequest request = new RecuperaDocumentoRequest();
            request.CodiceContratto = user.CodiceContrattoCOL;
            request.IdRichiesta = name.requestId;

            var dcs = service.RecuperaDocumento(request);
            var nameFile = "";

            if (dcs.Esito == EsitoPostaEvo.OK)
            {
                var n = DateTime.Now.Ticks + ".pdf";
                nameFile = "/public/download/" + n;
                var path = HttpContext.Current.Server.MapPath(nameFile);
                System.IO.File.WriteAllBytes(path, dcs.Documento.Contenuto);

                //SALVATAGGIO TEMPORANEO FILE
                //FINO A SETT/OTT 2020
                name.pathRecoveryFile = nameFile;
                _context.SaveChanges();

                GlobalClass.SendViaFtp(path, n, GlobalClass.usernameFtpDoc, GlobalClass.passwordFtpDoc, GlobalClass.ftpUrlDoc);
                File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("/public/download/"), n));
            }

            return nameFile;
        }
        public async Task<List<ResponseMOLState>> SetState(int namesId)
        {
            var r = new List<ResponseMOLState>();
            var nn = _context.Names.FirstOrDefault(a => a.id == namesId);
            if (nn == null)
                return null;

            var o = _context.Operations.FirstOrDefault(a => a.id == nn.operationId);
            if (o == null)
                return null;

            var user = _context.Users.FirstOrDefault(a => a.id == o.userId);
            if (user == null)
                return null;

            var guidUser = user.guidUser;

            PostaContestServiceClient service = getNewServiceCOL(guidUser);
            try
            {
                var s = await service.RecuperaEsitiPerIdRichiestaAsync(new RecuperaEsitiPerIdRichiestaRequest()
                {
                    IdRichieste = new[] { nn.requestId },
                    CodiceContratto = user.CodiceContrattoCOL
                });

                var n = _context.Names.FirstOrDefault(a => a.id == nn.id);

                if (s.Esito == EsitoPostaEvo.OK)
                {
                    n.finalState = false;
                    n.stato = s.RendicontazioneEsiti[0].DescrizioneEsito;
                    n.codice = s.RendicontazioneEsiti[0].CodiceTracciatura;
                    n.currentState = (int)currentState.InLavorazione;
                    if (s.RendicontazioneEsiti[0].CodiceEsito == "01" ||
                        s.RendicontazioneEsiti[0].CodiceEsito == "03" ||
                        s.RendicontazioneEsiti[0].CodiceEsito == "04")
                    {
                        if (s.RendicontazioneEsiti[0].CodiceEsito == "01")
                            n.consegnatoDate = s.RendicontazioneEsiti[0].DataEsito;

                        if (s.RendicontazioneEsiti[0].CodiceEsito == "03")
                            n.stato = s.RendicontazioneEsiti[0].DescrizioneEsito + " " + s.RendicontazioneEsiti[0].Causale;

                        n.finalState = true;

                        if (s.RendicontazioneEsiti[0].CodiceEsito == "04")
                            n.finalState = false;

                    }
                    else
                    {
                        var rr = s.RendicontazioneEsiti[0].DescrizioneEsito;
                        var cd = n.codice;
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                var error = ex;
            }

            return r;
        }


    }
}
