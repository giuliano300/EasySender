using Api.Dtos;
using Api.Models;
using Api.ServiceLol;
using AutoMapper;
using System;
using System.Collections.Generic;
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

        private static LOLServiceSoapClient getNewServiceLol(Guid guidUser)
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

        private static Destinatario GetDestinatarioLol(NameInsertDto name, int? index = 1)
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
            opzioniDiStampa.BW = (tipoStampa == tipoStampa.colori ? "true" : "false");
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

        private static Mittente GetMittente(SenderInsertDto sender)
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
            Nominativo.ComplementoIndirizzo = string.Empty;
            Nominativo.CasellaPostale = "";
            Nominativo.ComplementoNominativo = string.Empty;
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

        private void Abort(Richiesta[] Richiesta, Guid guid)
        {
            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guid);
            service.Annulla(Richiesta);
            service.Close();
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

        private ValidaDestinatariResults RecipientValidation(string IdRequest, Destinatario[] destinatari, Guid guid)
        {
            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guid);
            ValidaDestinatariResults esito = service.ValidaDestinatari(IdRequest, destinatari);
            service.Close();
            return esito;
        }

        [Route("RequestStatus")]
        [HttpGet]
        public GetStatoRichiesta GetStatusInviiIdRichiesta([FromUri]Guid guidUser, string requestId)
        {
            GetStatoRichiesta gsr = new GetStatoRichiesta();
            gsr.requestId = requestId;

            //MULTIPLE USERS
            var u = _context.Users.FirstOrDefault(a => a.guidUser == guidUser);
            if (u.id == 0)
            {
                gsr.statoDescrizione = "Nessun utente legato a questa richiesta.";
                return gsr;
            }

            Richiesta IdRol = new Richiesta();
            IdRol.GuidUtente = "";
            IdRol.IDRichiesta = requestId;

            Richiesta[] Richiesta = new Richiesta[1];
            Richiesta[0] = IdRol;

            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guidUser);

            GetStatoIdRichiestaResult stato = service.RecuperaStatoIdRichiesta(requestId);
            if (stato.StatoIdRichiesta == null)
            {
                gsr.statoDescrizione = "Nessun requestId corrispondente a quest'utenza";
                return gsr;
            }

            var sr = stato.StatoIdRichiesta;
            gsr.statoDescrizione = stato.StatoIdRichiesta;

            var n = _context.Names.Where(a => a.valid == true).FirstOrDefault(a => a.requestId == requestId);
            n.stato = gsr.statoDescrizione;

            var dto = Mapper.Map<Names, NamesDtos>(n);
            NamesController.UpdateItem(n.id, dto);

            return gsr;
        }

        private static Documento[] GetDocInsert(byte[] bite)
        {
            System.IO.FileInfo file;
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            Documento[] ArrayDocumento = new Documento[1];
            Documento documento = new Documento();
            file = new System.IO.FileInfo("/public/");
            documento.TipoDocumento = "pdf";
            documento.Immagine = bite;
            documento.MD5 = System.BitConverter.ToString(md5.ComputeHash(documento.Immagine)).Replace("-", string.Empty);
            ArrayDocumento[0] = documento;

            return ArrayDocumento;
        }

        [Route("Send")]
        [HttpPost]
        public async Task<Response> CheckName([FromBody] ObjectSenderNameLol senderRecipient, [FromUri] Guid guidUser)
        {
            var r = new Response();
            try
            {
                //MULTIPLE USERS
                var u = _context.Users.FirstOrDefault(a => a.guidUser == guidUser);

                //ERRORE GUID
                if (u.id == 0)
                {
                    r.state = "Utente non riconosciuto";
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
                    if (senderRecipient.recipient.state.ToUpper() == "ITALIA")
                    {
                        ControlloDestinatarioInsert crtlD = GlobalClass.verificaDestinatarioInsert(senderRecipient.recipient);
                        if (!crtlD.Valido)
                        {
                            r.state = crtlD.Errore;
                            return r;
                        }
                    }
                }
                else
                {
                    r.state = "Il destinatario non deve essere vuoto.";
                    return r;
                }


                LOLServiceSoapClient service = new LOLServiceSoapClient();
                service = getNewServiceLol(guidUser);

                var requestId = getRequestId(guidUser);

                var destinatari = new Destinatario[1];
                var d = new Destinatario();
                d = GetDestinatarioLol(senderRecipient.recipient);
                destinatari[0] = d;

                ValidaDestinatariResults rv = RecipientValidation(requestId, destinatari, guidUser);
                if (rv.CEResult.Type != "I")
                {
                    r.state = rv.CEResult.Description;
                    return r;
                }

                OperationsController oc = new OperationsController();
                OperationsDto op = new OperationsDto();
                op.date = DateTime.Now;
                op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
                op.userId = u.id;
                op.operationType = (int)operationType.LOL;
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

                createFeatures(ts, fr, operationId);

                SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(senderRecipient.sender);
                ss.operationId = operationId;
                int senderId = SenderController.CreateItem(ss);

                int idName = 0;
                var nc = new NamesController();
                NamesDtos nos = Mapper.Map<NameInsertDto, NamesDtos>(senderRecipient.recipient);
                nos.operationId = operationId;
                nos.valid = true;

                nos.fronteRetro = Convert.ToBoolean(fr);
                nos.tipoStampa = Convert.ToBoolean(ts);

                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

                //var filesName = new List<string>();
                //filesName.Add("F:/EasySender2.0/Api 5.0/Api/public/ewt/636957746461688600.pdf");

                DescrizioneLettera dl = new DescrizioneLettera();
                dl.TipoLettera = "Posta4";
                if (senderRecipient.tipoLettera == 1) { 
                    dl.TipoLettera = "Posta1";
                    dl.TracciaturaLettera = true;
                }

                LOLSubmit rs = new LOLSubmit();
                rs.DescrizioneLettera = dl;
                rs.Destinatari = destinatari;
                rs.Opzioni = GetOpzioniLol(ts, fr);
                rs.Mittente = GetMittente(senderRecipient.sender);
                rs.NumeroDestinatari = 1;
                rs.Documento = GetDocInsert(senderRecipient.recipient.attachedFile);

                rs.Nazionale = (senderRecipient.recipient.state.ToUpper() == "ITALIA" ? "true" : "false");

                InvioResult esito = new InvioResult();
                try
                {
                    esito = service.Invio(requestId, u.businessName, rs);

                    if (esito.CEResult.Type != "I")
                    {
                        r.state = esito.CEResult.Description;
                        return r;
                    }
                    nos.requestId = esito.IDRichiesta;
                    nos.guidUser = esito.GuidUtente;
                }
                catch (Exception e)
                {
                    r.state = "Errore nella richiesta del submit.";
                    return r;
                };

                Richiesta[] Richiesta = new Richiesta[1];
                Richiesta rich = new Richiesta();
                rich.GuidUtente = nos.guidUser;
                rich.IDRichiesta = nos.requestId;
                Richiesta[0] = rich;

                Thread.Sleep(5000);
                var v = await service.ValorizzaAsync(Richiesta);

                int i = 0;
                var st = v.ValorizzaResult.ServizioEnquiryResponse
                    .Where(a => a.StatoLavorazione.Id == "N"
                    || a.StatoLavorazione.Id == "Y"
                    || a.StatoLavorazione.Id == "J"
                    || a.StatoLavorazione.Id == "G"
                    || a.StatoLavorazione.Id == "R"
                    || a.StatoLavorazione.Id == "A"
                    || a.StatoLavorazione.Id == "U"
                    || a.StatoLavorazione.Id == "V"
                    || a.StatoLavorazione.Id == "W"
                    || a.StatoLavorazione.Id == "S"
                    )
                    .Count();

                if (st == 0)
                    do
                    {
                        Thread.Sleep(5000);
                        v = await service.ValorizzaAsync(Richiesta);
                        i++;


                        st = v.ValorizzaResult.ServizioEnquiryResponse
                            .Where(a => a.StatoLavorazione.Id == "N"
                            || a.StatoLavorazione.Id == "Y"
                            || a.StatoLavorazione.Id == "J"
                            || a.StatoLavorazione.Id == "G"
                            || a.StatoLavorazione.Id == "R"
                            || a.StatoLavorazione.Id == "A"
                            || a.StatoLavorazione.Id == "U"
                            || a.StatoLavorazione.Id == "V"
                            || a.StatoLavorazione.Id == "W"
                            || a.StatoLavorazione.Id == "S"
                           )
                        .Count();

                    } while (st == 0 && i < 50);

                if (v.ValorizzaResult.CEResult.Type != "I")
                {
                    r.state = v.ValorizzaResult.ServizioEnquiryResponse[0].StatoLavorazione.Descrizione;
                    return r;
                }

                var conferma = await service.PreConfermaAsync(Richiesta, true);
                if (conferma.PreConfermaResult.DestinatariLettera[0] != null)
                {
                    var ServizioEnquiryResponse = v.ValorizzaResult.ServizioEnquiryResponse.ToList();
                    var s = ServizioEnquiryResponse[0];

                    nos.guidUser = s.Richiesta.GuidUtente;
                    nos.orderId = conferma.PreConfermaResult.IdOrdine;
                    nos.price = Convert.ToDecimal(s.Totale.ImportoNetto);
                    nos.vatPrice = Convert.ToDecimal(s.Totale.ImportoIva);
                    nos.totalPrice = Convert.ToDecimal(s.Totale.ImportoTotale);
                    nos.currentState = (int)currentState.PresoInCarico;
                    nos.codice = conferma.PreConfermaResult.DestinatariLettera[0].IdRicevuta;

                }
                else
                {
                    r.state = "Errore nella conferma dell'invio";
                    return r;
                }


                var p = new Prices()
                {
                    price = nos.price,
                    vatPrice = nos.vatPrice,
                    totalPrice = nos.totalPrice
                };

                idName = nc.CreateItem(nos);

                r.userId = idName;
                r.valid = true;
                r.requestId = nos.requestId;
                r.guidUser = nos.guidUser;
                r.code = nos.codice;
                r.docImage = RequestDCS(guidUser, idName);
                r.state = "Inserimento effettuato con successo.";
                r.prices = p;
            }
            catch (Exception e)
            {
                r.state = e.Message.ToString();
            }
            return r;
        }

        [Route("SendBulletin")]
        [HttpPost]
        public async Task<Response> CheckNameBulletin([FromBody] ObjectSenderNameLolBulletin senderRecipient, [FromUri] Guid guidUser)
        {
            var r = new Response();
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


            //VERIFICA BOLLETTINO
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

            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guidUser);

            var requestId = getRequestId(guidUser);

            var destinatari = new Destinatario[1];
            var d = new Destinatario();
            d = GetDestinatarioLol(senderRecipient.recipient);
            destinatari[0] = d;

            ValidaDestinatariResults rv = RecipientValidation(requestId, destinatari, guidUser);
            if (rv.CEResult.Type != "I")
            {
                r.state = rv.CEResult.Description;
                return r;
            }

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.LOL;
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

            createFeatures(ts, fr, operationId);

            SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(senderRecipient.sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            int idName = 0;
            var nc = new NamesController();
            NamesDtos nos = Mapper.Map<NameInsertDto, NamesDtos>(senderRecipient.recipient);
            nos.operationId = operationId;
            nos.valid = true;

            nos.fronteRetro = Convert.ToBoolean(ts);
            nos.tipoStampa = Convert.ToBoolean(ts);

            nos.insertDate = DateTime.Now;
            nos.currentState = (int)currentState.inAttesa;

            //CREAZIONE BOLLETTINO
            int idBulletin = 0;
            var bc = new BulletinsController();
            BulletinsDto b = Mapper.Map<BulletinsDtos, BulletinsDto>(bs);

            PaginaBollettino pagina = new PaginaBollettino();
            object bo = null;
            bo = getBollettino896(bs);
            pagina.Bollettino = (Bollettino896)bo;
            PaginaBollettinoBase[] p = new PaginaBollettinoBase[1];
            p[0] = pagina;

            DescrizioneLettera dl = new DescrizioneLettera();
            dl.TipoLettera = "Posta 4";
            if (senderRecipient.tipoLettera == 1)
                dl.TipoLettera = "Posta 1";
            dl.TracciaturaLettera = true;

            LOLSubmit rs = new LOLSubmit();
            rs.DescrizioneLettera = dl;
            rs.Destinatari = destinatari;
            rs.Opzioni = GetOpzioniLol(ts, fr);
            rs.Mittente = GetMittente(senderRecipient.sender);
            rs.NumeroDestinatari = 1;
            rs.PagineBollettini = p;
            rs.Documento = GetDocInsert(senderRecipient.recipient.attachedFile);

            rs.Nazionale = (senderRecipient.recipient.state.ToUpper() == "ITALIA" ? "true" : "false");

            InvioResult esito = new InvioResult();
            try
            {
                esito = service.Invio(requestId, u.businessName, rs);

                if (esito.CEResult.Type != "I")
                {
                    r.state = "Errore nella validazione di poste";
                    return r;
                }
                nos.requestId = esito.IDRichiesta;
                nos.guidUser = esito.GuidUtente;
            }
            catch (Exception e)
            {
                r.state = "Errore nella richiesta del submit.";
                return r;
            };

            Richiesta[] Richiesta = new Richiesta[1];
            Richiesta rich = new Richiesta();
            rich.GuidUtente = nos.guidUser;
            rich.IDRichiesta = nos.requestId;
            Richiesta[0] = rich;

            Thread.Sleep(5000);
            var v = await service.ValorizzaAsync(Richiesta);

            int i = 0;
            var st = v.ValorizzaResult.ServizioEnquiryResponse
                .Where(a => a.StatoLavorazione.Id == "N"
                || a.StatoLavorazione.Id == "Y"
                || a.StatoLavorazione.Id == "J"
                || a.StatoLavorazione.Id == "G"
                || a.StatoLavorazione.Id == "R"
                || a.StatoLavorazione.Id == "A"
                || a.StatoLavorazione.Id == "U"
                || a.StatoLavorazione.Id == "V"
                || a.StatoLavorazione.Id == "W"
                || a.StatoLavorazione.Id == "S"
                )
                .Count();

            if (st == 0)
                do
                {
                    Thread.Sleep(5000);
                    v = await service.ValorizzaAsync(Richiesta);
                    i++;


                    st = v.ValorizzaResult.ServizioEnquiryResponse
                        .Where(a => a.StatoLavorazione.Id == "N"
                        || a.StatoLavorazione.Id == "Y"
                        || a.StatoLavorazione.Id == "J"
                        || a.StatoLavorazione.Id == "G"
                        || a.StatoLavorazione.Id == "R"
                        || a.StatoLavorazione.Id == "A"
                        || a.StatoLavorazione.Id == "U"
                        || a.StatoLavorazione.Id == "V"
                        || a.StatoLavorazione.Id == "W"
                        || a.StatoLavorazione.Id == "S"
                       )
                    .Count();

                } while (st == 0 && i < 50);

            if (v.ValorizzaResult.CEResult.Type != "I")
            {
                r.state = v.ValorizzaResult.ServizioEnquiryResponse[0].StatoLavorazione.Descrizione;
                return r;
            }

            var conferma = await service.PreConfermaAsync(Richiesta, true);
            if (conferma.PreConfermaResult.DestinatariLettera[0] != null)
            {
                var ServizioEnquiryResponse = v.ValorizzaResult.ServizioEnquiryResponse.ToList();
                var s = ServizioEnquiryResponse[0];

                nos.guidUser = s.Richiesta.GuidUtente;
                nos.orderId = conferma.PreConfermaResult.IdOrdine;
                nos.price = Convert.ToDecimal(s.Totale.ImportoNetto);
                nos.vatPrice = Convert.ToDecimal(s.Totale.ImportoIva);
                nos.totalPrice = Convert.ToDecimal(s.Totale.ImportoTotale);
                nos.currentState = (int)currentState.PresoInCarico;
                nos.codice = conferma.PreConfermaResult.DestinatariLettera[0].IdRicevuta;

            }
            else
            {
                r.state = "Errore nella conferma dell'invio";
                return r;
            }


            //CREAZIONE DESTINATARIO E BOLLETTINO
            idName = nc.CreateItem(nos);
            b.namesId = idName;
            idBulletin = bc.CreateItem(b);
 
            var price = new Prices()
            {
                price = nos.price,
                vatPrice = nos.vatPrice,
                totalPrice = nos.totalPrice
            };


            r.userId = idName;
            r.valid = true;
            r.requestId = nos.requestId;
            r.guidUser = nos.guidUser;
            r.code = nos.codice;
            r.docImage = RequestDCS(guidUser, idName);
            r.state = "Inserimento effettuato con successo.";
            r.prices = price;
            return r;
        }

        [Route("RequestDCS")]
        [HttpGet]
        public byte[] RequestDCS(Guid guidUser, int id)
        {
            LOLServiceSoapClient service = new LOLServiceSoapClient();
            service = getNewServiceLol(guidUser);

            var name = _context.Names.SingleOrDefault(a => a.id == id);

            Richiesta IdRol = new Richiesta();
            IdRol.GuidUtente = name.guidUser;
            IdRol.IDRichiesta = name.requestId;

            var dcs = service.RecuperaDCS(IdRol);
            byte[] nameFile = null;

            if (dcs.CEResult.Type == "I")
                nameFile = dcs.Documento.Immagine;

            return nameFile;
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

    }
}
