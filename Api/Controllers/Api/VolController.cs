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
using Api.Dtos;
using Api.Models;
using Api.ServiceVol;
using AutoMapper;
using Newtonsoft.Json;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Vol")]
    public class VolController : ApiController
    {
        private static Entities _context;

        public VolController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("ServiceVol")]
        [HttpGet]
        public static CameraliServiceClient getNewServiceVol(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;
            CameraliServiceClient service = new CameraliServiceClient();
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


        [Route("CheckAllFiles")]
        [HttpPost]
        public async Task<GetNumberOfCheckedNames> CheckAllFiles([FromUri] Guid guidUser, [FromBody] ObjectSubmit senderRecipients, [FromUri] int CodiceDocumento,
    [FromUri] int TipoDocumento, [FromUri] bool rrc, [FromUri] int userId)
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
            ControlloMittente ctrlM = GlobalClass.verificaMittente(sender);
            if (!ctrlM.Valido)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Mittente non valido";
                return ncn;
            }

            //ERRORE MITTENTE AR
            if (senderRecipients.senderAR != null)
            {
                ControlloMittente ctrlMAR = GlobalClass.verificaMittente(senderRecipients.senderAR);
                if (!ctrlMAR.Valido)
                {
                    ncn.numberOfValidNames = 0;
                    ncn.state = "Destinatario AR non valido";
                    return ncn;
                }
            }

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.VOL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = false;
            op.formatoSpeciale = false;
            int operationId = OperationsController.CreateItem(op);

            ricevutaRitorno rr = ricevutaRitorno.si;
            if (rrc)
                rr = ricevutaRitorno.no;


            createFeatures(tipoStampa.biancoNero, fronteRetro.fronte, rr, operationId);

            SenderDtos ss = Mapper.Map<SenderDto, SenderDtos>(sender);
            ss.operationId = operationId;
            ss.AR = false;
            int senderId = SenderController.CreateItem(ss);

            //CREAZIONE MITTENTE AR
            if (senderRecipients.senderAR != null)
            {
                SenderDtos ssAR = Mapper.Map<SenderDto, SenderDtos>(senderRecipients.senderAR);
                ssAR.operationId = operationId;
                ssAR.AR = true;
                int senderIdAR = SenderController.CreateItem(ssAR);
            }


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
                nos.requestId = null;
                nos.guidUser = null;
                nos.valid = true;

                nos.fronteRetro = false;
                nos.ricevutaRitorno = rrc;
                nos.tipoStampa = false;

                //VOL CAMPI SPECIFICI
                //CODICE CHISURA F(STATICO)
                nos.tipoDocumento = TipoDocumento;
                nos.codiceDocumento = CodiceDocumento;
                nos.codiceChiusura = "F";

                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

                //NUMERO DI PAGINE
                nos.numberOfPages = 0;

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
                    price =  GlobalClass.GetPriceVol(operationType.VOL, (CodiceDocumento)Enum.ToObject(typeof(CodiceDocumento), (int)CodiceDocumento))
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

        public async Task<string> RequestDCS(string requestId, Guid guidUser)
        {
            try
            {
                var name = _context.Names.FirstOrDefault(a => a.requestId == requestId);
                CameraliServiceClient service = new CameraliServiceClient();
                service = getNewServiceVol(guidUser);

                var dcs = service.RecuperaDocumento(new RecuperaDocumentoRequest()
                {
                    IdRichiesta = requestId
                });
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
            catch (Exception e)
            {
                return "";
            }
        }

        [Route("RequestDCSFor")]
        [HttpGet]

        public async Task<List<string>> RequestDCSFor(List<string> lRequestId, Guid guidUser)
        {
            try
            {
                Thread.Sleep(15000);
                var l = new List<string>();
                CameraliServiceClient service = new CameraliServiceClient();
                service = getNewServiceVol(guidUser);

                foreach(var requestId in lRequestId) { 

                    var wait = false;
                    var name = _context.Names.FirstOrDefault(a => a.requestId == requestId);
                    if (name.tipoDocumento == (int)TipoCamerale.Certificato)
                        wait = true;
  
                    var nameFile = "";

                    var dcs = service.RecuperaDocumento(new RecuperaDocumentoRequest()
                    {
                        IdRichiesta = requestId
                    });

                    int i = 0;
                    if(!wait)
                        do
                        {
                            Thread.Sleep(5000);
                            dcs = service.RecuperaDocumento(new RecuperaDocumentoRequest()
                            {
                                IdRichiesta = requestId
                            });

                            if (dcs.Errori.Length == 0)
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

                                wait = true;
                            }
                            i++;

                        } while (!wait && i < 20);

                    l.Add(nameFile);
                 }
                return l;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [Route("RetriveService")]
        [HttpGet]
        public async Task<List<string>> RetriveService(List<string> listRequestId, Guid guidUser)
        {

            List<string> validState = new List<string>();
            if (listRequestId == null)
                return null;
            try
            {
                Thread.Sleep(30000);

                CameraliServiceClient service = getNewServiceVol(guidUser);

                foreach (var idRichiesta in listRequestId)
                {
                    var wait = false;

                    var response = await service.RecuperaServizioAsync(new RecuperaServizioRequest
                    {
                        IdRichieste = new[] { idRichiesta }
                    });

                    if (response.Esito == EsitoPostaEvo.KO)
                        wait = true;

                    if (!wait && response.Servizi[0].StatoLavorazione.ToUpper().Replace(" ", "") == "COMPLETATA")
                        wait = true;

                    int i = 0;
                    if (!wait)
                        do
                        {
                            Thread.Sleep(5000);

                            response = await service.RecuperaServizioAsync(new RecuperaServizioRequest
                            {
                                IdRichieste = new[] { idRichiesta }
                            });

                            wait = response.Esito == EsitoPostaEvo.OK && response.Servizi[0].StatoLavorazione.ToUpper().Replace(" ", "") != "COMPLETATA";
                            i++;
                        } while (wait && i < 50);

                    var nn = _context.Names.SingleOrDefault(a => a.requestId == idRichiesta);

                    switch (response.Esito)
                    {
                        case EsitoPostaEvo.KO:
                            nn.currentState = (int)currentState.ErroreConfirm;
                            nn.stato = response.Errori[0].Messaggio.ToString();
                            nn.price = null;
                            nn.vatPrice = null;
                            nn.totalPrice = null;
                            nn.valid = false;
                            break;

                        case EsitoPostaEvo.OK:
                            if (response.Servizi[0].StatoLavorazione.ToUpper().Replace(" ", "") != "COMPLETATA")
                            {
                                nn.currentState = (int)currentState.ErroreStatoAtteso;
                                nn.price = null;
                                nn.vatPrice = null;
                                nn.totalPrice = null;
                                nn.valid = false;
                                _context.SaveChanges();
                            }
                            else
                            {
                                nn.currentState = (int)currentState.PresoInCarico;
                                _context.SaveChanges();
                                await RequestDCS(idRichiesta, guidUser);
                            }
                            break;

                    }

                }
            }
            catch (Exception e)
            {

                //GESTIONE ECCEZIONE
                //CREAZIONE LOG
                GlobalClass.CreateTxtFile("Errore durante recupera servizio VOL\n"
                    + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\n" + e.Message.ToString() + "\n", "RECUPERA_SERVIZIO-" + DateTime.Now.ToString("dd-MM-yyy"), "VOL");

                return null;
            }
            return validState;
        }

        private Richiedente GetRichiedente(SenderDto sender)
        {
            var richiedente = new Richiedente()
            {
                Nome = sender.name,
                Cognome = sender.surname,
                Indirizzo = sender.address,
                Localita = sender.city,
                CAP = sender.cap,
                Telefono = sender.telefono // Nuovo parametro
            };

            return richiedente;
        }
        private Intestatario GetIntestatario(NamesDto n, int codiceDocumento, int tipoDocumento)
        {
            var i = new Intestatario() { 
                    CCIAA = n.province.ToUpper(),
                    RagioneSociale = n.businessName + (n.name != " " ? " " + n.name : "") + (n.surname != " " ? " " + n.surname : "")
            };

            //EVOLUZIONI DATI FISCALI E NREA IN BASE AL TIPO DI DOCUMENTO
            switch ((TipoDocumento)Enum.ToObject(typeof(TipoDocumento), (int)tipoDocumento))
            {
                case TipoDocumento.CertificatoCamerale:
                    i.DatoFiscale = n.fiscalCode.Replace("IT", "").ToUpper();
                    i.NREA = n.NREA; //Numero REA, nuovo parametro
                    break;

                case TipoDocumento.VisuraCamerale:
                    String datoFiscale = null;
                    int? NREA = null;
                    switch ((CodiceDocumento)Enum.ToObject(typeof(CodiceDocumento), (int)codiceDocumento)){
                        case CodiceDocumento.SCPE:
                            datoFiscale = n.fiscalCode.Replace("IT", "").ToUpper();
                            break;
                        case CodiceDocumento.SCSC:
                        case CodiceDocumento.FASC:
                        case CodiceDocumento.BICM:
                            NREA = n.NREA;
                            break;
                        default:
                            datoFiscale = n.fiscalCode.Replace("IT", "").ToUpper();
                            NREA = n.NREA;
                            break;
                    }

                    i.DatoFiscale = datoFiscale;
                    i.NREA = NREA; //Numero REA, nuovo parametro
                    break;
            }

            return i;
        }

        private Destinatario GetDestinatario(SenderDto sender, TipoCamerale tipo  = TipoCamerale.Certificato)
        {
            //CERTIFICATO SOLO S DISPONIBILE(POSTA PRIORITARIA LOL1)
            //VISURA IMPOSTIAMO E
            TipoRecapito t = TipoRecapito.S;
            if (tipo == TipoCamerale.Visura)
                t = TipoRecapito.E;

            var destinatario = new Destinatario()
            {
                TipoRecapito = t,
                Nominativo = sender.businessName + (sender.name != " " ? " " + sender.name : "") + (sender.surname != " " ? " " + sender.surname : ""),
                Indirizzo = sender.address,
                Localita = sender.city,
                CAP = sender.cap,
                Email = sender.email //Nuovo parametro
            };

            return destinatario;
        }
    }
}
