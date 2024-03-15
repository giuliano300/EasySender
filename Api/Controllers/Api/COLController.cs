using Api.Dtos;
using Api.Models;
using Api.ServiceCOL;
using AutoMapper;
using System;
using System.Collections.Generic;
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
            opzioniStampa.FronteRetro = (fronteRetro == fronteRetro.fronte ? true : false);

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

        private static Mittente GetMittente(SenderInsertDto sender)
        {
            Mittente m = new Mittente();

            m.Nominativo = sender.businessName + " " + sender.name + " " + sender.surname;
            m.ComplementoIndirizzo = "";
            m.ComplementoNominativo = "";
            m.Indirizzo = sender.dug + " " + sender.address + " " + sender.houseNumber;
            m.Cap = sender.cap;
            m.Comune = sender.city;
            m.Provincia = sender.province;
            m.Nazione = sender.state;

            return m;
        }

        private static Bollettino896 getBollettino896(BulletinsDtos bollettino)
        {
            Bollettino896 b = new Bollettino896();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.FormatoStampa = 0;
            b.AdditionalInfo = "";
            b.IBAN = "";
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

        private static Bollettino451 getBollettino451(BulletinsDtos bollettino)
        {
            Bollettino451 b = new Bollettino451();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.FormatoStampa = 0;
            b.AdditionalInfo = "";
            b.IBAN = "";
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.ImportoEuro = bollettino.importoEuro;
            b.Causale = bollettino.causale;
            return b;
        }

        private static Bollettino674 getBollettino674(BulletinsDtos bollettino)
        {
            Bollettino674 b = new Bollettino674();
            b.NumeroContoCorrente = bollettino.numeroContoCorrente;
            b.IntestatoA = bollettino.intestatoA;
            b.FormatoStampa = 0;
            b.AdditionalInfo = "";
            b.IBAN = "";
            b.EseguitoDa = new BollettinoEseguitoDa();
            b.EseguitoDa.Nominativo = bollettino.eseguitoDaNominativo;
            b.EseguitoDa.Indirizzo = bollettino.eseguitoDaIndirizzo;
            b.EseguitoDa.CAP = bollettino.eseguitoDaCAP;
            b.EseguitoDa.Localita = bollettino.eseguitoDaLocalita;
            b.CodiceCliente = bollettino.codiceCliente;
            b.Causale = bollettino.causale;
            return b;
        }

        private static Documento[] getDoc(List<string> strNomeFile, int NumeroDiDocumenti)
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

        private void createFeatures(tipoStampa tipoStampa, fronteRetro fronteRetro, ricevutaRitorno ricevutaRitorno, int operationId)
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

            var op3 = new operationFeatures();
            op3.operationId = operationId;
            op3.featureType = "ricevuta Ritorno";
            op3.featureValue = Enum.GetName(typeof(ricevutaRitorno), ricevutaRitorno);
            _context.operationFeatures.Add(op3);
            _context.SaveChanges();

        }

        private static Destinatario GetDestinatarioMOL(NameInsertDto name, int? index = 1)
        {
            Destinatario n = new Destinatario();
            n.Nominativo = name.businessName + " " + name.surname + " " + name.name;
            n.ComplementoNominativo = name.complementNames;
            n.ComplementoIndirizzo = name.complementAddress;
            n.Indirizzo = name.dug + " " + name.address + " " + name.houseNumber;
            n.Cap = name.cap;
            n.Comune = name.city;
            n.Provincia = name.province;
            n.Nazione = "ITALIA";

            return n;
        }

        [Route("Send")]
        [HttpPost]
        public async Task<ResponseMOLCOL> CheckName([FromBody] ObjectSenderNameLol senderRecipient, [FromUri] Guid guidUser)
        {
            var r = new ResponseMOLCOL();
            //MULTIPLE USERS
            var u = _context.Users.FirstOrDefault(a => a.guidUser == guidUser);

            //ERRORE GUID
            if (u.id == 0)
            {
                r.state = "Utente non riconosiuto";
                return r;
            }

            //VERIFICA MITTENTE
            if (senderRecipient.sender != null)
            {
                ControlloMittenteInsert ctrlM = GlobalClass.verificaMittenteInsert(senderRecipient.sender);
                if (!ctrlM.Valido)
                {
                    r.state = ctrlM.Errore;
                    return r;
                }
            }
            else
            {
                r.state = "Il mittente non deve essere vuoto.";
                return r;
            }


            //VERIFICA DESTINATARIO
            if (senderRecipient.recipient != null)
            {
                ControlloDestinatarioInsert crtlD = GlobalClass.verificaDestinatarioInsert(senderRecipient.recipient);
                if (!crtlD.Valido)
                {
                    r.state = crtlD.Errore;
                    return r;
                }
            }
            else
            {
                r.state = "Il destinatario non deve essere vuoto.";
                return r;
            }

            PostaContestServiceClient service = getNewServiceCOL(guidUser);

            Destinatario d = new Destinatario();
            d = GetDestinatarioMOL(senderRecipient.recipient);
            Destinatario[] ld = new Destinatario[1];
            ld[0] = d;

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.COL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = true;
            int operationId = OperationsController.CreateItem(op);

            tipoStampa ts = tipoStampa.colori;
            if (senderRecipient.tipoStampa == (int)tipoStampa.biancoNero)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (senderRecipient.fronteRetro == (int)fronteRetro.fronteRetro)
                fr = fronteRetro.fronteRetro;


            SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(senderRecipient.sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            int idName = 0;
            var nc = new NamesController();
            NamesDtos n = Mapper.Map<NameInsertDto, NamesDtos>(senderRecipient.recipient);
            n.operationId = operationId;
            n.valid = true;

            n.fronteRetro = Convert.ToBoolean(fr);
            n.ricevutaRitorno = false;
            n.tipoStampa = Convert.ToBoolean(ts);

            n.insertDate = DateTime.Now;
            n.currentState = (int)currentState.inAttesa;

            idName = nc.CreateItem(n);
            var nn = _context.Names.SingleOrDefault(a => a.id == idName);

            var request = new InvioRequest();

            var intestazione = new Intestazione();
            intestazione.CodiceContratto = u.CodiceContrattoCOL;
            intestazione.Prodotto = ProdottoPostaEvo.COL1;

            //var filesName = new List<string>();
            //filesName.Add("F:/EasySender2.0/Api 5.0/Api/public/ewt/636957746461688600.pdf");


            var lettera = new PostaContest();
            lettera.AutoConferma = false;
            lettera.Destinatari = ld;
            lettera.Opzioni = GetOpzioni(ts, fr);
            lettera.Mittente = GetMittente(senderRecipient.sender);
            lettera.Documenti = GetDocInsert(senderRecipient.recipient.attachedFile);

            request.Intestazione = intestazione;
            request.PostaContest = lettera;

            try
            {
                var esito = service.Invio(request);

                if (esito.Esito == EsitoPostaEvo.OK)
                {
                    nn.requestId = esito.IdRichiesta;
                    nn.valid = true;
                    _context.SaveChanges();

                    Thread.Sleep(5000);

                    var c = Confirm(esito.IdRichiesta, guidUser);
                    if (c.EsitoPostaEvo == EsitoPostaEvo.KO)
                    {
                        nn.stato = "Errore nella conferma di poste. Ritentare l'invio.";
                        nn.requestId = null;
                        _context.SaveChanges();

                        r.valid = false;
                        r.requestId = null;
                        r.code = null;
                        r.state = "Errore nella conferma di poste. Ritentare l'invio.";
                        return r;
                    }

                }
                if (esito.Esito == EsitoPostaEvo.KO)
                {
                    nn.stato = "Errore nella validazione di poste";
                    nn.valid = false;
                    _context.SaveChanges();
                }

            }
            catch (Exception e)
            {
                nn.valid = false;
                nn.stato = "Errore nella richiesta del submit.";
                _context.SaveChanges();

                r.valid = false;
                r.requestId = null;
                r.code = null;
                r.state = "Errore nella richiesta del submit.";
                return r;
            };


            r.userId = idName;
            r.valid = true;
            r.requestId = nn.requestId;
            r.code = nn.codice;
            r.docImage = RequestDCSImg(guidUser, idName);
            r.state = "Inserimento effettuato con successo.";
            return r;
        }

        [Route("SendBulletin")]
        [HttpPost]
        public async Task<ResponseMOLCOL> CheckNameBulletin([FromBody] ObjectSenderNameLolBulletin senderRecipient, [FromUri] Guid guidUser)
        {
            var r = new ResponseMOLCOL();
            //MULTIPLE USERS
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            //ERRORE GUID
            if (u.id == 0)
            {
                r.state = "Utente non riconosiuto";
                return r;
            }

            //VERIFICA MITTENTE
            if (senderRecipient.sender != null)
            {
                ControlloMittenteInsert ctrlM = GlobalClass.verificaMittenteInsert(senderRecipient.sender);
                if (!ctrlM.Valido)
                {
                    r.state = ctrlM.Errore;
                    return r;
                }
            }
            else
            {
                r.state = "Il mittente non deve essere vuoto.";
                return r;
            }


            //VERIFICA DESTINATARIO
            if (senderRecipient.recipient != null)
            {
                ControlloDestinatarioInsert crtlD = GlobalClass.verificaDestinatarioInsert(senderRecipient.recipient);
                if (!crtlD.Valido)
                {
                    r.state = crtlD.Errore;
                    return r;
                }
            }
            else
            {
                r.state = "Il destinatario non deve essere vuoto.";
                return r;
            }

            BulletinsDtos bs = null;
            if (senderRecipient.bulletin != null)
            {
                bs = Mapper.Map<BulletinsApiDto, BulletinsDtos>(senderRecipient.bulletin);
                bs.bulletinType = (int)bulletinType.Bollettino896;
                ControlloBollettino crtlD = GlobalClass.verificaBollettino(bs);
                if (!crtlD.Valido)
                {
                    r.state = crtlD.Errore;
                    return r;
                }
            }
            else
            {
                r.state = "Il bollettino non deve essere vuoto.";
                return r;
            }

            PostaContestServiceClient service = getNewServiceCOL(guidUser);

            Destinatario d = new Destinatario();
            d = GetDestinatarioMOL(senderRecipient.recipient);
            Destinatario[] ld = new Destinatario[1];
            ld[0] = d;

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.COL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = true;
            int operationId = OperationsController.CreateItem(op);

            tipoStampa ts = tipoStampa.colori;
            if (senderRecipient.tipoStampa == (int)tipoStampa.biancoNero)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (senderRecipient.fronteRetro == (int)fronteRetro.fronteRetro)
                fr = fronteRetro.fronteRetro;


            SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(senderRecipient.sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            int idName = 0;
            var nc = new NamesController();
            NamesDtos n = Mapper.Map<NameInsertDto, NamesDtos>(senderRecipient.recipient);
            n.operationId = operationId;
            n.valid = true;

            n.fronteRetro = Convert.ToBoolean(ts);
            n.ricevutaRitorno = false;
            n.tipoStampa = Convert.ToBoolean(ts);

            n.insertDate = DateTime.Now;
            n.currentState = (int)currentState.inAttesa;

            idName = nc.CreateItem(n);
            var nn = _context.Names.SingleOrDefault(a => a.id == idName);

            var request = new InvioRequest();

            var intestazione = new Intestazione();
            intestazione.CodiceContratto = u.CodiceContrattoCOL;
            intestazione.Prodotto = ProdottoPostaEvo.COL1;

            var lettera = new PostaContest();
            lettera.AutoConferma = false;
            lettera.Destinatari = ld;
            lettera.Opzioni = GetOpzioni(ts, fr);
            lettera.Mittente = GetMittente(senderRecipient.sender);
            lettera.Documenti = GetDocInsert(senderRecipient.recipient.attachedFile);

            request.Intestazione = intestazione;
            request.PostaContest = lettera;


            PaginaBollettino pagina = new PaginaBollettino();
            object b = null;
            b = getBollettino896(Mapper.Map<BulletinsApiDto, BulletinsDtos>(senderRecipient.bulletin));
            pagina.Bollettino = (Bollettino896)b;
            PaginaBollettino[] p = new PaginaBollettino[1];
            p[0] = pagina;
            request.PostaContest.Bollettini = p;


            try
            {
                var esito = service.Invio(request);

                if (esito.Esito == EsitoPostaEvo.OK)
                {
                    nn.requestId = esito.IdRichiesta;
                    nn.valid = true;
                    _context.SaveChanges();

                    Thread.Sleep(5000);

                    var c = Confirm(esito.IdRichiesta, guidUser);
                    if (c.EsitoPostaEvo == EsitoPostaEvo.KO)
                    {
                        nn.stato = "Errore nella conferma di poste. Ritentare l'invio.";
                        nn.requestId = null;
                        _context.SaveChanges();

                        r.valid = false;
                        r.requestId = null;
                        r.code = null;
                        r.state = "Errore nella conferma di poste. Ritentare l'invio.";
                        return r;
                    }

                }
                if (esito.Esito == EsitoPostaEvo.KO)
                {
                    nn.stato = "Errore nella validazione di poste";
                    nn.valid = false;
                    _context.SaveChanges();
                }

            }
            catch (Exception e)
            {
                nn.valid = false;
                nn.stato = "Errore nella richiesta del submit.";
                _context.SaveChanges();

                r.valid = false;
                r.requestId = null;
                r.code = null;
                r.state = "Errore nella richiesta del submit.";
                return r;
            };


            r.userId = idName;
            r.valid = true;
            r.requestId = nn.requestId;
            r.code = nn.codice;
            r.docImage = RequestDCSImg(guidUser, idName);
            r.state = "Inserimento effettuato con successo.";
            return r;
        }

        private ResponseMOLState GetState(string requestId, Guid guidUser)
        {
            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            PostaContestServiceClient service = getNewServiceCOL(guidUser);
            var request = new RecuperaStatoRequest();
            request.CodiceContratto = u.CodiceContrattoCOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseMOLState();

            var rs = service.RecuperaStato(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                var s = rs.StatoInvii[0];
                r.stato = s.CodiceStatoRichiesta;
                r.descrizioneStato = s.DescrizioneStatoRichiesta;
                r.dataUltimaModifica = s.DataUltimaModifica.ToString();

                var l = GlobalClass.ListOfState();
                var state = l.SingleOrDefault(a => a.identificativo == s.CodiceStatoRichiesta);

                if(state != null) {
                    if (state.tipologia.ToUpper() != "DEFINITIVO") { 
                        var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                        if (!state.state)
                            n.valid = false;

                        n.stato = s.DescrizioneStatoRichiesta;
                        _context.SaveChanges();
                    }
                }
            }
            return r;
        }

        public GetStatoRichiesta GetStatusInviiIdRichiesta([FromUri] Guid guidUser, string requestId, string codice = null)
        {
            GetStatoRichiesta gsr = new GetStatoRichiesta();
            gsr.requestId = requestId;
            gsr.numeroServizio = codice;
            gsr.numeroServizio = codice;

            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            PostaContestServiceClient service = getNewServiceCOL(guidUser);
            var request = new RecuperaEsitiPerIdRichiestaRequest();
            request.CodiceContratto = u.CodiceContrattoCOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseCOLConfirm();

            var rs = service.RecuperaEsitiPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                var res = rs.RendicontazioneEsiti[0];
                gsr.statoDescrizione = res.DescrizioneEsito;
                gsr.dataEsito = res.DataEsito.ToString("dd/MM/yyyy");
                if(res.CodiceEsito == "01")
                    gsr.finale = true;
            }
            else
            {
                var e = rs.Errori[0];
                gsr.statoDescrizione = e.Messaggio.ToString();
            }
            return gsr;
        }


        private ResponseCOLConfirm StateRetrive(string requestId, Guid guidUser)
        {
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            PostaContestServiceClient service = getNewServiceCOL(guidUser);
            var request = new RecuperaServizioPerIdRichiestaRequest();
            request.CodiceContratto = u.CodiceContrattoCOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseCOLConfirm();

            var rs = service.RecuperaServizioPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                if (rs.Servizi.Count() > 0)
                {
                    if(rs.Servizi[0].DataAccettazione != null && rs.Servizi[0].DatiServizio.Destinatari[0].NumeroLettera != null) { 
                        var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                        n.presaInCaricoDate = rs.Servizi[0].DataAccettazione;
                        n.codice = rs.Servizi[0].DatiServizio.Destinatari[0].NumeroLettera.Replace(" ", "");
                        n.stato = rs.Servizi[0].StatoServizio;
                        _context.SaveChanges();

                        r.DataAccettazione = (DateTime)rs.Servizi[0].DataAccettazione;
                        r.NumeroLettera = rs.Servizi[0].DatiServizio.Destinatari[0].NumeroLettera.Replace(" ", "");
                        r.EsitoPostaEvo = rs.Esito;
                    }
                }
            }
            return r;
        }

        [Route("RequestStatus")]
        [HttpGet]
        public ResponseCOLConfirm ResultRetrive(string requestId, Guid guidUser)
        {
            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            PostaContestServiceClient service = getNewServiceCOL(guidUser);
            var request = new RecuperaEsitiPerIdRichiestaRequest();
            request.CodiceContratto = u.CodiceContrattoCOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseCOLConfirm();

            var rs = service.RecuperaEsitiPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                r.EsitoPostaEvo = rs.Esito;
                r.NumeroLettera = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                r.DataAccettazione = rs.RendicontazioneEsiti[0].DataAccettazione;

                var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                n.presaInCaricoDate = rs.RendicontazioneEsiti[0].DataAccettazione;
                n.codice = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                _context.SaveChanges();

            }
            return r;
        }

        private ResponseCOLConfirm Confirm(string requestId, Guid guidUser)
        {
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            var r = new ResponseCOLConfirm();
            PostaContestServiceClient service = getNewServiceCOL(guidUser);
            var request = new ConfermaInvioRequest();
            request.CodiceContratto = u.CodiceContrattoCOL;
            request.IdRichiesta = requestId;

            var stato = StateRetrive(requestId, guidUser);
            if (stato.EsitoPostaEvo == EsitoPostaEvo.OK)
            {
                r.DataAccettazione = (DateTime)stato.DataAccettazione;
                r.NumeroLettera = stato.NumeroLettera.Replace(" ", "");
                r.EsitoPostaEvo = stato.EsitoPostaEvo;
            }
            else
            {
                var conferma = service.ConfermaInvio(request);
                if(conferma.Esito == EsitoPostaEvo.OK) { 
                    r.DataAccettazione = conferma.DataAccettazione;
                    r.NumeroLettera = conferma.Destinatari[0].NumeroLettera;
                    r.EsitoPostaEvo = conferma.Esito;

                    var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                    n.presaInCaricoDate = r.DataAccettazione;
                    n.codice = r.NumeroLettera;
                    n.stato = "Presa in carico Postel";
                    n.currentState = (int)currentState.PresoInCarico;
                    _context.SaveChanges();
                }
            }

            return r;
        }

        private List<GetStatoRichiesta> RequestOperationStatus(Guid guidUser, int operationId)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            List<GetStatoRichiesta> lGsr = new List<GetStatoRichiesta>();
            var n = _context.Names.Where(a => a.operationId == operationId).Where(a => a.requestId != null).ToList();
            if (n.Count() == 0)
                return null;

            int id = n[0].operationId;
            var o = _context.Operations.SingleOrDefault(a => a.id == id);

            foreach (var name in n)
                {
                    var s = GetState(name.requestId, guidUser);
                    GetStatoRichiesta gsr = new GetStatoRichiesta();
                    gsr.requestId = name.requestId;

                    gsr.statoDescrizione = s.descrizioneStato;
                    gsr.numeroServizio = name.codice;
                    gsr.dataEsito = s.dataUltimaModifica;

                    //name.consegnatoDate = Convert.ToDateTime(gsr.dataEsito);

                    lGsr.Add(gsr);
                }

            return lGsr;
        }

        private GetStatoRichiesta RequestNameStatus(Guid guidUser, Names name)
        {

            var s = GetState(name.requestId, guidUser);
            GetStatoRichiesta gsr = new GetStatoRichiesta();
            gsr.requestId = name.requestId;

            gsr.statoDescrizione = s.descrizioneStato;
            gsr.numeroServizio = name.codice;
            gsr.dataEsito = s.dataUltimaModifica;

            return gsr;
        }

        private string RequestDCS(Guid guidUser, int id)
        {
            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            PostaContestServiceClient service = new PostaContestServiceClient();
            service = getNewServiceCOL(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            RecuperaDocumentoRequest request = new RecuperaDocumentoRequest();
            request.CodiceContratto = u.CodiceContrattoCOL;
            request.IdRichiesta = name.requestId;

            var dcs = service.RecuperaDocumento(request);
            var nameFile = "";

            if (dcs.Esito == EsitoPostaEvo.OK)
            {
                nameFile = "/public/download/" + DateTime.Now.Ticks + ".pdf";
                var path = HttpContext.Current.Server.MapPath(nameFile);
                System.IO.File.WriteAllBytes(path, dcs.Documento.Contenuto);
            }

            return nameFile;
        }

        private byte[] RequestDCSImg(Guid guidUser, int id)
        {
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            PostaContestServiceClient service = new PostaContestServiceClient();
            service = getNewServiceCOL(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            RecuperaDocumentoRequest request = new RecuperaDocumentoRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
            request.IdRichiesta = name.requestId;

            var dcs = service.RecuperaDocumento(request);
            byte[] nameFile = null;

            if (dcs.Esito == EsitoPostaEvo.OK)
                nameFile = dcs.Documento.Contenuto;

            return nameFile;
        }

        private static Documento[] GetDocInsert(byte[] bite)
        {
            Documento[] ArrayDocumento = new Documento[1];
            try 
            { 
                System.IO.FileInfo file;
                System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

                Documento documento = new Documento();
                file = new System.IO.FileInfo("/public/");
                documento.Estensione = "pdf";
                documento.Contenuto = bite;
                documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(documento.Contenuto)).Replace("-", string.Empty);
                ArrayDocumento[0] = documento;

            }
            catch(Exception e) 
            {
                var ex = e;
            };
            return ArrayDocumento;
        }

    }
}
