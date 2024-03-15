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

        private static Mittente GetMittente(SenderInsertDto sender)
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

        [Route("Send")]
        [HttpPost]
        public GetSubmitResponse Submit([FromUri]Guid guidUser, [FromBody] ObjectSubmitTelegram senderRecipientTelegram)
        {
            GetSubmitResponse sr = new GetSubmitResponse();

            //MULTIPLE USERS
            var users = _context.Users.Where(a => a.guidUser == guidUser).ToList();

            //ERRORE GUID
            if (users.Count() == 0)
            {
                sr.state = (int)state.preControlError;
                sr.message = "GUID inesistente";
                return sr;
            }


            //ERRORE MITTENTE
            ControlloMittenteInsert ctrlM = GlobalClass.verificaMittenteInsert(senderRecipientTelegram.sender);
            if (!ctrlM.Valido)
            {
                sr.state = (int)state.preControlError;
                sr.message = ctrlM.Errore;
                return sr;
            }

            //ERRORE TESTO
            if (senderRecipientTelegram.testo == "")
            {
                sr.state = (int)state.preControlError;
                sr.message = "Inserire il testo del telegramma";
                return sr;
            }

            WCFTelegrammiServiceClient service = new WCFTelegrammiServiceClient();
            service = getNewServiceTelegram(guidUser);

            SenderInsertDto sender = senderRecipientTelegram.sender;
            NamesTelegramDto recipient = senderRecipientTelegram.recipient;
            string text = senderRecipientTelegram.testo;

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
            op.userId = users[0].id;
            op.operationType = (int)operationType.TELEGRAMMA;
            op.complete = true;
            int operationId = OperationsController.CreateItem(op);

            SenderDtos ss = Mapper.Map<SenderInsertDto, SenderDtos>(sender);
            ss.operationId = operationId;
            int senderId = SenderController.CreateItem(ss);

            var requestId = GetIdNewTelegram(guidUser);

            NamesDtos nos = Mapper.Map<NamesTelegramDto, NamesDtos>(recipient);
            nos.operationId = operationId;
            nos.requestId = requestId;
            nos.valid = true;
            nos.insertDate = DateTime.Now;

            ControlloDestinatario ctrl = GlobalClass.verificaDestinatarioTelegramma(recipient);
            if (!ctrl.Valido)
            {
                sr.state = (int)state.preControlError;
                sr.message = ctrl.Errore;

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
                    esito = service.Submit(ref Telegramm, users[0].businessName, requestId);
                }
                catch (Exception e)
                {
                    return sr;
                }
                if(esito.Result.ResType.ToString() != "I")
                {
                    sr.state = (int)state.error;
                    sr.message = esito.Result.Description;
                    sr.requestId = requestId;
                    sr.orderId = null;
                }
                else
                {
                    string[] IdReq = new string[1];
                    IdReq[0] = requestId;

                    sr.state = (int)state.valid;
                    sr.message = "Success";
                    sr.requestId = requestId;
                    PreconfirmResult pr = null;
                    try
                    {
                        pr = service.PreConfirm(IdReq, true, true);
                    }
                    catch (Exception e)
                    {
                        return sr;
                    }

                    if (pr.Result.ResType.ToString() != "I")
                    {
                        sr.state = (int)state.error;
                        sr.message = pr.Result.Description;
                        sr.requestId = requestId;
                        sr.orderId = null;
                    }
                    else
                    {
                        sr.state = (int)currentState.PresoInCarico;
                        sr.message = "Preso in carico";
                        sr.code = pr.ConfirmOrderResponse.TicketIds[0].ID;
                        nos.codice = pr.ConfirmOrderResponse.TicketIds[0].ID;

                        Prices prices = new Prices();
                        prices.price = Convert.ToDecimal(pr.OrderResponse.Total.NetValue);
                        prices.vatPrice = Convert.ToDecimal(pr.OrderResponse.Total.TaxAmount);
                        prices.totalPrice = Convert.ToDecimal(pr.OrderResponse.Total.GrossValue);
                        sr.orderId = pr.OrderResponse.IdOrder;
                        sr.prices = prices;

                        nos.orderId = sr.orderId;
                        nos.price = Convert.ToDecimal(prices.price);
                        nos.vatPrice = Convert.ToDecimal(prices.vatPrice);
                        nos.totalPrice = Convert.ToDecimal(prices.totalPrice);
                        nos.currentState = (int)currentState.PresoInCarico;
                    }
                            
                }

                nos.fileName = "";
                var nc = new NamesController();
                int idName = 0;
                try
                {
                    idName = nc.CreateItem(nos);
                }catch(Exception ex)
                {
                    var x = ex;
                };

            }

            service.Close();
            return sr;
        }

        private GetOperationResponse Confirm(Guid guidUser, int operationId, int userId = 0)
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

                PreconfirmResult pr = service.PreConfirm(IdReq, true, true);

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

    }
}
