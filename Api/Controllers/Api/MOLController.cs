using Api.Dtos;
using Api.Models;
using Api.ServiceMOL;
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
    [RoutePrefix("api/MOL")]
    public class MOLController : ApiController
    {
        private static Entities _context;

        public MOLController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        private static Opzioni GetOpzioni(tipoStampa tipoStampa, fronteRetro fronteRetro, ricevutaRitorno ricevutaRitorno)
        {
            var opzioni = new Opzioni();

            var opzioniStampa = new OpzioniStampa();
            opzioniStampa.TipoColore = (tipoStampa == tipoStampa.colori ? TipoColore.COLORE : TipoColore.BW);
            opzioniStampa.FronteRetro = (fronteRetro == fronteRetro.fronte ? true : false);

            var servizio = new OpzioniServizio();
            servizio.Consegna = ModalitaConsegna.S;
            servizio.AttestazioneConsegna = false;

            if (ricevutaRitorno == ricevutaRitorno.si)
                servizio.AttestazioneConsegna = true;

            opzioni.Stampa = opzioniStampa;
            opzioni.Servizio = servizio;

            return opzioni;
        }

        private static RaccomandataMarketServiceClient getNewServiceMOL(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;
            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
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

        private static DestinatarioAR GetDestinatarioARMOL(SenderInsertDto name, int? index = 1)
        {
            DestinatarioAR n = new DestinatarioAR();
            n.Nominativo = name.businessName + " " + name.surname + " " + name.name;
            n.ComplementoNominativo = "";
            n.ComplementoIndirizzo = "";
            n.Indirizzo = name.dug + " " + name.address + " " + name.houseNumber;
            n.Cap = name.cap;
            n.Comune = name.city;
            n.Provincia = name.province;
            n.Nazione = "ITALIA";

            return n;
        }

        [Route("Send")]
        [HttpPost]
        public async Task<ResponseMOLCOL> CheckName([FromBody] ObjectSenderName senderRecipient, [FromUri] Guid guidUser)
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

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);

            Destinatario d = new Destinatario();
            d = GetDestinatarioMOL(senderRecipient.recipient);
            Destinatario[] ld = new Destinatario[1];
            ld[0] = d;

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.MOL;
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

            ricevutaRitorno rr = ricevutaRitorno.si;
            if (senderRecipient.ricevutaRitorno == (int)ricevutaRitorno.no)
                rr = ricevutaRitorno.no;


            SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(senderRecipient.sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            int idName = 0;
            var nc = new NamesController();
            NamesDtos n = Mapper.Map<NameInsertDto, NamesDtos>(senderRecipient.recipient);
            n.operationId = operationId;
            n.valid = true;

            n.fronteRetro = Convert.ToBoolean(fr);
            n.ricevutaRitorno = Convert.ToBoolean(rr);
            n.tipoStampa = Convert.ToBoolean(ts);

            n.insertDate = DateTime.Now;
            n.currentState = (int)currentState.inAttesa;

            idName = nc.CreateItem(n);
            var nn = _context.Names.SingleOrDefault(a => a.id == idName);

            var request = new InvioRequest();

            var intestazione = new Intestazione();
            intestazione.CodiceContratto =  u.CodiceContrattoMOL;
            intestazione.Prodotto = ProdottoPostaEvo.MOL1;

            var market = new MarketOnline();
            market.AutoConferma = true;
            market.Destinatari = ld;
            market.Opzioni = GetOpzioni(ts, fr, rr);
            market.Mittente = GetMittente(senderRecipient.sender);
            market.Documenti = GetDocInsert(senderRecipient.recipient.attachedFile);

            if (rr == ricevutaRitorno.si)
            {
                if (senderRecipient.senderAR == null)
                    market.DestinatarioAR = GetDestinatarioARMOL(senderRecipient.sender);
                else
                    market.DestinatarioAR = GetDestinatarioARMOL(senderRecipient.senderAR);
            }

            request.Intestazione = intestazione;
            request.MarketOnline = market;

            try
            {
                var esito = service.Invio(request);

                if (esito.Esito == EsitoPostaEvo.OK)
                {
                    nn.requestId = esito.IdRichiesta;
                    nn.valid = true;
                    _context.SaveChanges();

                }
                if (esito.Esito == EsitoPostaEvo.KO)
                {
                    nn.stato = esito.Errori[0].Messaggio.ToString();
                    nn.valid = false;
                    _context.SaveChanges();

                    r.valid = false;
                    r.requestId = null;
                    r.code = null;
                    r.state = esito.Errori[0].Messaggio.ToString();
                    return r;
                }

            }
            catch (Exception e)
            {
                nn.valid = false;
                nn.stato = e.Message.ToString();
                _context.SaveChanges();

                r.valid = false;
                r.requestId = null;
                r.code = null;
                r.state = e.Message.ToString();
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
        public async Task<ResponseMOLCOL> CheckNameBulletin([FromBody] ObjectSenderNameBulletin senderRecipient, [FromUri] Guid guidUser)
        {
            var r = new ResponseMOLCOL();
            try { 
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
                };

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

                RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);

                Destinatario d = new Destinatario();
                d = GetDestinatarioMOL(senderRecipient.recipient);
                Destinatario[] ld = new Destinatario[1];
                ld[0] = d;

                OperationsController oc = new OperationsController();
                OperationsDto op = new OperationsDto();
                op.date = DateTime.Now;
                op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
                op.userId = u.id;
                op.operationType = (int)operationType.MOL;
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

                ricevutaRitorno rr = ricevutaRitorno.si;
                if (senderRecipient.ricevutaRitorno == (int)ricevutaRitorno.no)
                    rr = ricevutaRitorno.no;


                SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(senderRecipient.sender);
                ss.operationId = operationId;
                int senderId = SenderController.CreateItem(ss);

                int idName = 0;
                var nc = new NamesController();
                NamesDtos n = Mapper.Map<NameInsertDto, NamesDtos>(senderRecipient.recipient);
                n.operationId = operationId;
                n.valid = true;

                n.fronteRetro = Convert.ToBoolean(ts);
                n.ricevutaRitorno = Convert.ToBoolean(rr);
                n.tipoStampa = Convert.ToBoolean(ts);

                n.insertDate = DateTime.Now;
                n.currentState = (int)currentState.inAttesa;

                idName = nc.CreateItem(n);
                var nn = _context.Names.SingleOrDefault(a => a.id == idName);

                var request = new InvioRequest();

                var intestazione = new Intestazione();
                intestazione.CodiceContratto = u.CodiceContrattoMOL;
                intestazione.Prodotto = ProdottoPostaEvo.MOL1;

                var market = new MarketOnline();
                market.AutoConferma = true;
                market.Destinatari = ld;
                market.Opzioni = GetOpzioni(ts, fr, rr);
                market.Mittente = GetMittente(senderRecipient.sender);
                market.Documenti = GetDocInsert(senderRecipient.recipient.attachedFile);

                if (rr == ricevutaRitorno.si)
                {
                    if (senderRecipient.senderAR == null)
                        market.DestinatarioAR = GetDestinatarioARMOL(senderRecipient.sender);
                    else
                        market.DestinatarioAR = GetDestinatarioARMOL(senderRecipient.senderAR);
                }

                request.Intestazione = intestazione;
                request.MarketOnline = market;

                PaginaBollettino pagina = new PaginaBollettino();
                object b = null;
                b = getBollettino896(Mapper.Map<BulletinsApiDto,BulletinsDtos>(senderRecipient.bulletin));
                pagina.Bollettino = (Bollettino896)b;
                PaginaBollettino[] p = new PaginaBollettino[1];
                p[0] = pagina;
                request.MarketOnline.Bollettini = p;

                try
                {
                    var esito = service.Invio(request);

                    if (esito.Esito == EsitoPostaEvo.OK)
                    {
                        nn.requestId = esito.IdRichiesta;
                        nn.valid = true;
                        _context.SaveChanges();

                    }
                    if (esito.Esito == EsitoPostaEvo.KO)
                    {
                        nn.stato = esito.Errori[0].Messaggio.ToString();
                        nn.valid = false;
                        _context.SaveChanges();

                        r.valid = false;
                        r.requestId = null;
                        r.code = null;
                        r.state = esito.Errori[0].Messaggio.ToString();
                        return r;
                    }

                }
                catch (Exception e)
                {
                    nn.valid = false;
                    nn.state = e.Message.ToString();
                    _context.SaveChanges();

                    r.valid = false;
                    r.requestId = null;
                    r.code = null;
                    r.state = e.Message.ToString();
                    return r;
                };


                r.userId = idName;
                r.valid = true;
                r.requestId = nn.requestId;
                r.code = nn.codice;
                r.docImage = RequestDCSImg(guidUser, idName);
                r.state = "Inserimento effettuato con successo.";
            }
            catch(Exception e)
            {
                r.valid = false;
                r.state = e.Message.ToString();
            }
            return r;
        }

        private ResponseMOLState GetState(string requestId, Guid guidUser)
        {
            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaStatoRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
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

            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaEsitiPerIdRichiestaRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseCOLConfirm();

            var rs = service.RecuperaEsitiPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                var res = rs.RendicontazioneEsiti[0];
                gsr.statoDescrizione = res.DescrizioneEsito;
                gsr.dataEsito = res.DataEsito.ToString("dd/MM/yyyy");
                if (res.CodiceEsito == "01")
                    gsr.finale = true;
            }
            else
            {
                var e = rs.Errori[0];
                gsr.statoDescrizione = e.Messaggio.ToString();
            }
            return gsr;
        }


        private ResponseMOLConfirm StateRetrive(string requestId, Guid guidUser)
        {
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaServizioPerIdRichiestaRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseMOLConfirm();

            var rs = service.RecuperaServizioPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                if (rs.Servizi.Count() > 0)
                {
                    if(rs.Servizi[0].DataAccettazione != null && rs.Servizi[0].DatiServizio.Destinatari[0].NumeroRaccomandata != null) { 
                        var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                        n.presaInCaricoDate = rs.Servizi[0].DataAccettazione;
                        n.codice = rs.Servizi[0].DatiServizio.Destinatari[0].NumeroRaccomandata.Replace(" ", "");
                        n.stato = rs.Servizi[0].StatoServizio;
                        _context.SaveChanges();

                        r.DataAccettazione = (DateTime)rs.Servizi[0].DataAccettazione;
                        r.NumeroRaccomandata = rs.Servizi[0].DatiServizio.Destinatari[0].NumeroRaccomandata.Replace(" ", "");
                        r.EsitoPostaEvo = rs.Esito;
                    }
                }
            }
            return r;
        }

        [Route("RequestStatus")]
        [HttpGet]
        public ResponseMOLConfirm ResultRetrive(string requestId, Guid guidUser)
        {
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            string[] IdRichieste = new string[1];
            IdRichieste[0] = requestId;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaEsitiPerIdRichiestaRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
            request.IdRichieste = IdRichieste;

            var r = new ResponseMOLConfirm();

            var rs = service.RecuperaEsitiPerIdRichiesta(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                r.EsitoPostaEvo = rs.Esito;
                r.NumeroRaccomandata = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                r.DataAccettazione = rs.RendicontazioneEsiti[0].DataAccettazione;

                var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                n.presaInCaricoDate = rs.RendicontazioneEsiti[0].DataAccettazione;
                n.codice = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                _context.SaveChanges();

            }
            return r;
        }

        [Route("RetriveService")]
        [HttpGet]
        public async Task<ResponseMOLState> RetriveService(string requestId, int userId)
        {
            var r = new ResponseMOLState();
            var user = _context.Users.SingleOrDefault(a => a.id == userId);
            var guidUser = user.guidUser;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var services = await service.RecuperaServizioPerIdRichiestaAsync(
                new RecuperaServizioPerIdRichiestaRequest
                {
                    CodiceContratto = user.CodiceContrattoMOL,
                    IdRichieste = new[] { requestId }
                });

            if (services.Servizi != null)
            {
                var postalizzata = services.Servizi[0];
                var IdRichiesta = postalizzata.DatiServizio.IdRichiesta;
                var n = _context.Names.SingleOrDefault(a => a.requestId == IdRichiesta);

                //Recupero il Numero Lettera
                var numeroLettera = postalizzata.DatiServizio.Destinatari[0].NumeroRaccomandata;
                if (postalizzata.ValorizzazioneServizio != null)
                {
                    var ImportoNettoServizio = postalizzata.ValorizzazioneServizio.ImportoNettoServizio;
                    n.price = Convert.ToDecimal(ImportoNettoServizio);
                    n.totalPrice = Convert.ToDecimal(ImportoNettoServizio);
                }
                n.stato = postalizzata.StatoServizio.Replace("Postel", "Poste");
                n.currentState = (int)currentState.InLavorazione;
                n.codice = numeroLettera;
                _context.SaveChanges();

                 var rl = new ResponseMOLState()
                {
                    dataUltimaModifica = postalizzata.DataAccettazione.Value.ToString(),
                    descrizioneStato = postalizzata.StatoServizio.Replace("Postel", "Poste"),
                    stato = "L"
                };
                return rl;

           }
            r.stato = "Nessun dato disponibile";
            return r;
        }


        private ResponseMOLConfirm StateRetriveForCode(string code, Guid guidUser)
        {
            var u = _context.Users
                .Where(a => a.parentId == 0)
                .FirstOrDefault(a => a.guidUser == guidUser);

            string[] codes = new string[1];
            codes[0] = code;

            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new RecuperaServizioPerNumeroRaccomandataRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
            request.NumeroRaccomandate = codes;


            var r = new ResponseMOLConfirm();

            var rs = service.RecuperaServizioPerNumeroRaccomandata(request);
            if (rs.Esito == EsitoPostaEvo.OK)
            {
                //r.EsitoPostaEvo = rs.Esito;
                //r.NumeroRaccomandata = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                //r.DataAccettazione = rs.RendicontazioneEsiti[0].DataAccettazione;

                //var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                //n.presaInCaricoDate = rs.RendicontazioneEsiti[0].DataAccettazione;
                //n.codice = rs.RendicontazioneEsiti[0].CodiceTracciatura.Replace(" ", "");
                //_context.SaveChanges();

            }
            return r;
        }

        private ResponseMOLConfirm Confirm(string requestId, Guid guidUser)
        {
            var u = _context.Users
               .Where(a => a.parentId == 0)
               .FirstOrDefault(a => a.guidUser == guidUser);

            var r = new ResponseMOLConfirm();
            RaccomandataMarketServiceClient service = getNewServiceMOL(guidUser);
            var request = new ConfermaInvioRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
            request.IdRichiesta = requestId;

            var stato = StateRetrive(requestId, guidUser);
            if (stato.EsitoPostaEvo == EsitoPostaEvo.OK)
            {
                r.DataAccettazione = (DateTime)stato.DataAccettazione;
                r.NumeroRaccomandata = stato.NumeroRaccomandata.Replace(" ", "");
                r.EsitoPostaEvo = stato.EsitoPostaEvo;
            }
            else
            {
                var conferma = service.ConfermaInvio(request);
                if(conferma.Esito == EsitoPostaEvo.OK) { 
                    r.DataAccettazione = conferma.DataAccettazione;
                    r.NumeroRaccomandata = conferma.DestinatariRaccomandate[0].NumeroRaccomandata;
                    r.EsitoPostaEvo = conferma.Esito;

                    var n = _context.Names.SingleOrDefault(a => a.requestId == requestId);
                    n.presaInCaricoDate = r.DataAccettazione;
                    n.codice = r.NumeroRaccomandata;
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

            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            service = getNewServiceMOL(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            RecuperaDocumentoRequest request = new RecuperaDocumentoRequest();
            request.CodiceContratto = u.CodiceContrattoMOL;
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

            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            service = getNewServiceMOL(guidUser);

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
            System.IO.FileInfo file;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Documento[] ArrayDocumento = new Documento[1];
            Documento documento = new Documento();
            file = new System.IO.FileInfo("/public/");
            documento.Estensione = "pdf";
            documento.Contenuto = bite;
            documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(documento.Contenuto)).Replace("-", string.Empty);
            ArrayDocumento[0] = documento;

            return ArrayDocumento;
        }

    }
}
