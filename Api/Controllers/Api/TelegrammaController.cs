using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Api.Models;
using Api.ServiceTelegrammi;
using System.Web.Http.Description;
using Api.Dtos;
using System.Threading.Tasks;
using AutoMapper;
using Api.DataModel;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Telegram")]
    public class TelegrammaController : ApiController
    {
        private Entities _context;

        public TelegrammaController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        private WCFTelegrammiServiceClient getNewServiceTelegram(Guid GuidProprieta)
        {
            var Users = _context.Users.FirstOrDefault(p => p.guidUser == GuidProprieta);
            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
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

        private string GetIdNewTelegram(Guid guidUser)
        {
            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);
            var IdRichiesta = service.GetIdRequest().ToString();
            service.Close();

            return IdRichiesta;
        }

        private static string getStaticRequestId()
        {
            var IdRichiesta = Guid.NewGuid();
            return IdRichiesta.ToString();
        }

        private static Mittente GetMittente(SenderDto sender)
        {

            Mittente Mittente = new Mittente();
            Mittente.Nome = sender.name;
            Mittente.Cognome = sender.surname;
            Mittente.Indirizzo = sender.dug + " " + sender.address + " " + sender.houseNumber;
            Mittente.CAP = sender.cap;
            Mittente.Citta = sender.city;
            Mittente.InvioAlMittente = false;
            Mittente.Telefono = string.Empty;
            return Mittente;
        }

        private static Destinatario GetDestinatarioTelegramma(NamesTelegramDto name, int? index = 1)
        {
            Destinatario d = new Destinatario();
            d.RagioneSociale = name.businessName;
            d.Nome = name.name;
            d.Cognome = name.surname;
            d.Indirizzo = name.dug + " " + name.address + " " + name.houseNumber;
            d.CAP = name.cap;
            d.Citta = name.city;
            d.Stato = name.state;
            return d;
        }

        private SubmitResult StaticSubmit()
        {
            TResult tr = new TResult()
            {
                ResType = TResultResType.I,
                ResDetails = ""
            };

            SubmitResult s = new SubmitResult()
            {
                DestinatariResult = null,
                ExtensionData = null,
                Result = tr
            };

            return s;
        }

        private PreconfirmResult StaticPreconfirm()
        {
            TResult tr = new TResult()
            {
                ResType = TResultResType.I,
                ResDetails = ""
            };

            var totale = GlobalClass.GeneraImportoStatico();
            GranTotal gt = new GranTotal()
            {
               TaxAmount = Convert.ToDecimal(totale.ImportoIva),
               NetValue = Convert.ToDecimal(totale.ImportoNetto),
               GrossValue = Convert.ToDecimal(totale.ImportoTotale)
            };

            OrderResponse or = new OrderResponse()
            {
               IdOrder = GlobalClass.GeneraOrdineStatico(),
               Total = gt
            };

            TicketIdsTicketId[] id = new TicketIdsTicketId[1];
            id[0] = new TicketIdsTicketId();
            id[0].ID = GlobalClass.GeneraNumeroLetteraStatico();

            ConfirmOrderResponse o = new ConfirmOrderResponse()
            {
                TicketIds = id
            };

            PreconfirmResult s = new PreconfirmResult()
            {
               OrderResponse  = or,
               ConfirmOrderResponse  = o,
               Result = tr
            };

            return s;
        }

        [Route("Submit")]
        [HttpPost]
        public GetOperationResponse Submit([FromUri]Guid guidUser, [FromBody] ObjectSubmitTelegram senderRecipientsTelegram, [FromUri]bool RicevutaRitorno = false, [FromUri]bool autoConfirm = false, 
            [FromUri]int userId = 0)
        {
            GetOperationResponse gor = new GetOperationResponse();
            List<GetSubmitResponse> lGsr = new List<GetSubmitResponse>();

            //MULTIPLE USERS
            var users = _context.Users.Where(a => a.guidUser == guidUser);

            //ERRORE GUID
            if (users.Count() == 0)
            {
                gor.state = (int)state.preControlError;
                gor.message = "GUID inesistente";
                lGsr = null;
                return gor;
            }

            //UTENTE INSERITORE
            var u = new Users();
            if (userId > 0)
                u = users.SingleOrDefault(a => a.id == userId);
            else
                u = users.SingleOrDefault(a => a.parentId == 0);

            //ERRORE MITTENTE
            ControlloMittente ctrlM = GlobalClass.verificaMittente(senderRecipientsTelegram.sender);
            if (!ctrlM.Valido)
            {
                gor.state = (int)state.preControlError;
                gor.message = ctrlM.Errore;
                lGsr = null;
                return gor;
            }

            //ERRORE TESTO
            if (senderRecipientsTelegram.testo == "")
            {
                gor.state = (int)state.preControlError;
                gor.message = "Inserire il testo del telegramma";
                lGsr = null;
                return gor;
            }

            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);

            SenderDto sender = new SenderDto();
            sender = senderRecipientsTelegram.sender;
            List<NamesTelegramDto> recipients = senderRecipientsTelegram.recipients;
            string text = senderRecipientsTelegram.testo;

            Opzioni OpzioniTelegramma = new Opzioni();

            ServiceTelegrammi.Telegramma Telegramm = new ServiceTelegrammi.Telegramma();

            InfoTesto info = new InfoTesto();
            info.Testo = text;
            info.NumeroParteCorrente = 1;
            int i = 1;


            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = "Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.areaTestOperation = u.areaTestUser;
            op.operationType = (int)operationType.TELEGRAMMA;
            int operationId = OperationsController.CreateItem(op);

            SenderDtos ss = Mapper.Map<SenderDto, SenderDtos>(sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            foreach (var recipient in recipients)
            {
                GetSubmitResponse gsr = new GetSubmitResponse();
                var requestId = "";
                if (!u.demoUser)
                    requestId = GetIdNewTelegram(u.guidUser);
                else
                    requestId = getStaticRequestId();

                NamesDtos nos = Mapper.Map<NamesTelegramDto, NamesDtos>(recipient);
                nos.operationId = operationId;
                nos.requestId = requestId;
                nos.valid = true;
                nos.insertDate = DateTime.Now;

                ControlloDestinatario ctrl = GlobalClass.verificaDestinatarioTelegramma(recipient);
                if (!ctrl.Valido)
                {
                    gsr.state = (int)state.preControlError;
                    gsr.message = ctrl.Errore;
                    lGsr.Add(gsr);

                    nos.valid = false;
                    nos.currentState = (int)currentState.Annullato;
                }
                else
                {
                    TelegrammaDestinatario TelegrammaDestinatario = new TelegrammaDestinatario();
                    TelegrammaDestinatario.Destinatario = GetDestinatarioTelegramma(recipient);
                    TelegrammaDestinatario.NumeroDestinatarioCorrente = i;
                    TelegrammaDestinatario.IDTelegramma = string.Empty;
                    TelegrammaDestinatario.Frazionario = string.Empty;
                    TelegrammaDestinatario.LineaPilota = string.Empty;
                    TelegrammaDestinatario.TipoRec = TelegrammaDestinatarioTipoRec.Item;

                    TelegrammaDestinatario[] TelegrammaDestinatari = new TelegrammaDestinatario[1];
                    TelegrammaDestinatari[0] = TelegrammaDestinatario;

                    Telegramm.GUIDMessage = string.Empty;
                    OpzioniTelegramma.CTA = false;
                    OpzioniTelegramma.Note = string.Empty;
                    Telegramm.Firma = string.Empty;
                    Telegramm.DataTelegramma = DateTime.Now;
                    Telegramm.Mittente = GetMittente(sender);
                    Telegramm.Nazionale = (sender.state.ToLower() == "italia" ? true : false);
                    Telegramm.Opzioni = OpzioniTelegramma;
                    Telegramm.Destinatari = TelegrammaDestinatari;
                    Telegramm.PartiTesto = info;
                    SubmitResult esito = null;

                    try
                    {
                        if(!u.demoUser)
                            esito = service.Submit(ref Telegramm, u.businessName, requestId);
                        else
                            esito = StaticSubmit();
                    }
                    catch (Exception e)
                    {
                        return gor;
                    }
                    if(esito.Result.ResType.ToString() != "I")
                    {
                        gsr.state = (int)state.error;
                        gsr.message = esito.Result.Description;
                        gsr.requestId = requestId;
                        gsr.orderId = null;
                        lGsr.Add(gsr);
                    }
                    else
                    {
                        string[] IdReq = new string[1];
                        IdReq[0] = requestId;

                        gsr.state = (int)state.valid;
                        gsr.message = "Success";
                        gsr.requestId = requestId;
                        PreconfirmResult pr = null;
                        try
                        {
                            if (!u.demoUser)
                                pr = service.PreConfirm(IdReq, autoConfirm, true);
                            else
                                pr = StaticPreconfirm();
                        }
                        catch (Exception e)
                        {
                            return gor;
                        }

                        if (pr.Result.ResType.ToString() != "I")
                        {
                            gsr.state = (int)state.error;
                            gsr.message = pr.Result.Description;
                            gsr.requestId = requestId;
                            gsr.orderId = null;
                        }
                        else
                        {
                            if(autoConfirm)
                            {
                                gsr.state = (int)currentState.PresoInCarico;
                                gsr.message = "Preso in carico";
                                gsr.code = pr.ConfirmOrderResponse.TicketIds[0].ID;
                                nos.codice = pr.ConfirmOrderResponse.TicketIds[0].ID;
                            }

                            Prices prices = new Prices();
                            prices.price = Convert.ToDecimal(pr.OrderResponse.Total.NetValue);
                            prices.vatPrice = Convert.ToDecimal(pr.OrderResponse.Total.TaxAmount);
                            prices.totalPrice = Convert.ToDecimal(pr.OrderResponse.Total.GrossValue);
                            gsr.orderId = pr.OrderResponse.IdOrder;
                            gsr.prices = prices;

                            nos.orderId = gsr.orderId;
                            nos.price = Convert.ToDecimal(gsr.prices.price);
                            nos.vatPrice = Convert.ToDecimal(gsr.prices.vatPrice);
                            nos.totalPrice = Convert.ToDecimal(gsr.prices.totalPrice);
                            nos.currentState = (int)currentState.PresoInCarico;
                        }
                            
                        lGsr.Add(gsr);

                    }
                    i++;


                }
                nos.fileName = "";
                var nc = new NamesController();
                int idName = nc.CreateItem(nos, u.userPriority);

            }

            gor.operationId = operationId;
            gor.state = (int)state.valid;
            gor.ListGetSubmitResponse = lGsr;

            service.Close();
            return gor;
        }

        [Route("Confirm")]
        [HttpGet]
        public GetOperationResponse Confirm(Guid guidUser, int operationId, int userId = 0)
        {
            GetOperationResponse gor = new GetOperationResponse();
            List<GetSubmitResponse> lGsr = new List<GetSubmitResponse>();

            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);

            var users = _context.Users.Where(x => x.guidUser == guidUser);
            if (users.Count() == 0)
                return null;

            //UTENTE INSERITORE
            var u = new Users();
            if (userId > 0)
                u = users.SingleOrDefault(a => a.id == userId);
            else
                u = users.SingleOrDefault(a => a.parentId == 0);

            var op = _context.Operations.Where(a=> a.Users.guidUser == guidUser).SingleOrDefault(a => a.id == operationId);
            if (op == null)
                return null;

            var n = _context.Names.Where(a => a.operationId == operationId).Where(a => a.codice == null).ToList();

            foreach(var name in n)
            {
                GetSubmitResponse gsr = new GetSubmitResponse();
                string[] IdReq = new string[1];
                IdReq[0] = name.requestId;


                PreconfirmResult pr = null;
                if (!u.demoUser)
                    pr = service.PreConfirm(IdReq, true, true);
                else
                    pr = StaticPreconfirm();

                gsr.requestId = name.requestId;
                gsr.orderId = name.orderId;
                gsr.code = name.codice;
                Prices prices = new Prices();
                prices.price = Convert.ToDecimal(name.price);
                prices.vatPrice = Convert.ToDecimal(name.vatPrice);
                prices.totalPrice = Convert.ToDecimal(name.totalPrice);
                gsr.prices = prices;

                if (pr.Result.ResType.ToString() != "I")
                {
                    gsr.state = (int)state.error;
                    gsr.message = pr.Result.Description;
                }
                else
                {
                    var nos = _context.Names.SingleOrDefault(a => a.requestId == name.requestId);
                    nos.orderId = pr.OrderResponse.IdOrder;
                    nos.codice = pr.ConfirmOrderResponse.TicketIds[0].ID;
                    nos.price = Convert.ToDecimal(pr.OrderResponse.Total.NetValue);
                    nos.vatPrice = Convert.ToDecimal(pr.OrderResponse.Total.TaxAmount);
                    nos.totalPrice = Convert.ToDecimal(pr.OrderResponse.Total.GrossValue);
                    nos.currentState = (int)currentState.PresoInCarico;

                    var nameDto = Mapper.Map<Names, NamesDtos>(nos);
                    NamesController.UpdateItem(nos.id, nameDto);

                    gsr.state = (int)currentState.PresoInCarico;
                    gsr.message = "Preso in carico";
                    gsr.code = pr.ConfirmOrderResponse.TicketIds[0].ID;
                }
                lGsr.Add(gsr);
            }

            gor.operationId = operationId;
            gor.state = (int)state.valid;
            gor.ListGetSubmitResponse = lGsr;

            service.Close();

            op.complete = true;
            _context.SaveChanges();

            return gor;
        }

        [Route("RequestStatus")]
        [HttpGet]
        public GetStatoRichiesta GetStatusInviiIdRichiesta(Guid guidUser, string IdRichiesta)
        {

            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            int[] ids = new int[u.Count()];
            int i = 0;
            foreach (var usr in u)
            {
                ids[i] = usr.id;
                i++;
            }

            GetStatoRichiesta gsr = new GetStatoRichiesta();
            gsr.requestId = IdRichiesta;
            var n = _context.Names.Where(a => ids.Contains(a.Operations.Users.id)).SingleOrDefault(a => a.requestId == IdRichiesta);
            if (n == null)
            {
                gsr.statoDescrizione = "Nessun destinatario corrispondente a questo IdRichiesta";
                return gsr;
            }

            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);

            GetStatusRequest GetStatusRequest = new GetStatusRequest();
            GetStatusRequest.GUIDMessage = IdRichiesta;
            GetStatusResult stato = service.GetStatus(GetStatusRequest);

            if (stato.Result.ResType.ToString() =="I")
            {
                gsr.statoDescrizione = stato.Result.Description;
                gsr.numeroServizio = n.codice;
                gsr.dataEsito = null;
            }

            return gsr;
        }

        [Route("RequestOperationStatus")]
        [HttpGet]
        public List<GetStatoRichiesta> RequestOperationStatus(Guid guidUser, int operationId)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            List<GetStatoRichiesta> lGsr = new List<GetStatoRichiesta>();
            var n = _context.Names.Where(a => a.operationId == operationId).Where(a => a.requestId != null).ToList();
            if (n.Count() == 0)
                return null;

            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);

            int id = n[0].operationId;
            var o = _context.Operations.SingleOrDefault(a => a.id == id);
            if (!o.demoOperation)
            {
                foreach (var name in n)
                {
                    GetStatoRichiesta gsr = new GetStatoRichiesta();
                    gsr.requestId = name.requestId;
                    var na = _context.Names.SingleOrDefault(a => a.requestId == name.requestId);
                    if (na == null)
                    {
                        gsr.statoDescrizione = "Nessun destinatario corrispondente a questo IdRichiesta";
                    }
                    else { 


                        GetStatusRequest GetStatusRequest = new GetStatusRequest();
                        GetStatusRequest.GUIDMessage = name.requestId;
                        GetStatusResult stato = service.GetStatus(GetStatusRequest);

                        if (stato.Result.ResType.ToString() == "I")
                        {
                            var description = "";
                            if (stato.Result.Description.Contains("Operation completed successfuly"))
                                description = "Operazione completata.";
                            else
                                description = stato.Result.Description;

                            gsr.statoDescrizione = description;
                            gsr.numeroServizio = na.codice;
                            gsr.dataEsito = null;
                        }
                    }

                    lGsr.Add(gsr);
               }
            }
            else
            {
                foreach (var name in n)
                {
                    GetStatoRichiesta gsr = new GetStatoRichiesta();
                    gsr.requestId = name.requestId;
                    gsr.statoDescrizione = "Consegnato";
                    gsr.numeroServizio = name.codice;
                    gsr.dataEsito = DateTime.Now.ToString();

                    name.stato = gsr.statoDescrizione;
                    name.consegnatoDate = Convert.ToDateTime(gsr.dataEsito);

                    var dto = Mapper.Map<Names, NamesDtos>(name);
                    NamesController.UpdateItem(name.id, dto);

                    lGsr.Add(gsr);
                }
            }
            return lGsr;
        }

        [Route("RequestNameStatus")]
        [HttpGet]
        public GetStatoRichiesta RequestNameStatus(Guid guidUser, Names name)
        {

            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);

            GetStatoRichiesta gsr = new GetStatoRichiesta();
            gsr.requestId = name.requestId;
            var na = _context.Names.SingleOrDefault(a => a.requestId == name.requestId);
            if (na == null)
            {
                gsr.statoDescrizione = "Nessun destinatario corrispondente a questo IdRichiesta";
            }
            else
            {

                GetStatusRequest GetStatusRequest = new GetStatusRequest();
                GetStatusRequest.GUIDMessage = name.requestId;
                GetStatusResult stato = service.GetStatus(GetStatusRequest);

                if (stato.Result.ResType.ToString() == "I")
                {
                    var description = "";
                    if (stato.Result.Description.Contains("Operation completed successfuly"))
                        description = "Operazione completata.";
                    else
                        description = stato.Result.Description;

                    gsr.statoDescrizione = description;
                    gsr.numeroServizio = na.codice;
                    gsr.dataEsito = null;
                }
            }

            return gsr;
        }


    }
}
