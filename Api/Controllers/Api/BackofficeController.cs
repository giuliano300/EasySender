using Api.Dtos;
using Api.Models;
using AutoMapper;
using GemBox.Email.Calendar;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Backoffice")]
    public class BackofficeController : ApiController
    {
        private Entities _context;

        public BackofficeController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("GetROLMOLCOLCount")]
        [HttpGet]
        public async Task<int> GetROLMOLCOLCount()
        {
            var n = _context.Names
                .Where(a => a.currentState == (int)currentState.PresoInCarico || a.currentState == (int)currentState.InLavorazione)
                .Where(a => a.valid == true)
                .Where(a => a.finalState == false)
                .Where(a => a.Operations.Users.abilitato != false)
                .Where(a => a.Operations.areaTestOperation == false)
                .Where(a =>
                    a.Operations.operationType == (int)operationType.ROL ||
                    a.Operations.operationType == (int)operationType.MOL ||
                    a.Operations.operationType == (int)operationType.COL
                );

            return n.Count();
        }

        [HttpGet]
        [Route("SplitSend")]
        public ObjectResult<GetExportableData_Result> SplitSender(string userId = null, string startDate = null, string endDate = null)
        {
            DateTime? e = null; DateTime? s = null; if (startDate != null)
                s = Convert.ToDateTime(startDate);
            if (endDate != null)
                e = Convert.ToDateTime(endDate); var t = _context.GetExportableData(s, e, userId); return t;
        }

        [HttpGet]
        [Route("")]
        public List<DisplayCounters> DisplayCounters(int userId = 0, string startDate = "", string endDate = "", string sendType = "", int propertyId = 0)
        {

            var l = new List<DisplayCounters>();

            var n = _context.Names
                .Where(a => a.Operations.areaTestOperation == false)
                .Where(a => a.Operations.complete == true)
                .Where(a => a.valid == true)
                .Where(a => a.currentState > 0);

            //UTENTE
            if (userId > 0)
                n = n.Where(a => a.Operations.userId == userId || a.Operations.userParentId == userId);

            //TIPO
            if (sendType != "")
            {
                var type = sendType.ToLower();
                switch (type)
                {
                    case "rol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.ROL);
                        break;
                    case "lol1":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.LOL).Where(a => a.tipoLettera == "Posta1" || a.tipoLettera == null);
                        break;
                    case "lol4":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.LOL).Where(a => a.tipoLettera == "Posta4");
                        break;
                    case "mol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.MOL);
                        break;
                    case "col":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.COL);
                        break;
                    case "tol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.TELEGRAMMA);
                        break;
                    case "vol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.VOL);
                        break;
                };
            }

            //DATA
            if (startDate != "")
            {
                var s = Convert.ToDateTime(startDate);
                n = n.Where(a => a.Operations.date >= s);
            }
            if (endDate != "")
            {
                var e = Convert.ToDateTime(endDate).AddDays(1);
                n = n.Where(a => a.Operations.date < e);
            }

            if (propertyId > 0)
            {
                n = n.Where(a => a.Operations.Users.porpertyId == propertyId);
            }

            if (userId == 0 && startDate == "" && endDate == "" && sendType == "" && propertyId == 0)
            {
                var dd = DateTime.Now.ToString();
                var ss = Convert.ToDateTime(dd);
                var es = Convert.ToDateTime(dd).AddDays(1);
                n = n.Where(a => a.Operations.date >= ss).Where(a => a.Operations.date < es);
            }

            var c = n.Count();
            if (c > 0)
            {
                var totalNames = n.GroupBy(a => a.Operations.Users.guidUser);
                foreach (var t in totalNames)
                {
                    try
                    {

                        var b = t.FirstOrDefault().Operations.Users.guidUser;
                        var r = _context.Users.Where(a => a.guidUser == b).FirstOrDefault(a => a.parentId == 0);
                        decimal totalPrice = (decimal)t.Where(a => a.totalPrice != null).Sum(a => a.totalPrice);
                        int totalGed = t.Where(a => a.pathGEDUrl != null && a.pathGEDUrl != "").Count();

                        var d = new DisplayCounters()
                        {
                            businessName = r.businessName != "" ? r.businessName : r.name + " " + r.lastName,
                            numberOfNames = t.Count(),
                            totalPrice = totalPrice,
                            totalGed = totalGed,
                            posteGed = "-"
                        };

                        if (r.usernameGED != null && r.usernameGED != "")
                            d.posteGed = "Si";

                        l.Add(d);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return l;
        }

        [HttpGet]
        [Route("Invii")]
        public List<DisplayInvii> DisplayInvii(int userId = 0, string startDate = "", string endDate = "", string sendType = "", string valid = "", string formato = "", string currentState = "", int propertyId = 0, bool? notificato = null, string noCod = null)
        {

            var l = new List<DisplayInvii>();

            var n = _context.Names
                .Where(a => a.Operations.areaTestOperation == false)
                .Where(a => a.Operations.complete == true);

            //UTENTE
            if (userId > 0)
                n = n.Where(a => a.Operations.userId == userId || a.Operations.userParentId == userId);

            //PROPRIETA'
            if (propertyId > 0)
                n = n.Where(a => a.Operations.Users.Property.id == propertyId);

            //TIPO
            if (sendType != "")
            {
                var type = sendType.ToLower();
                switch (type)
                {
                    case "rol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.ROL);
                        break;
                    case "lol1":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.LOL).Where(a => a.tipoLettera == "Posta1" || a.tipoLettera == null);
                        break;
                    case "lol4":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.LOL).Where(a => a.tipoLettera == "Posta4");
                        break;
                    case "mol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.MOL);
                        break;
                    case "col":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.COL);
                        break;
                    case "tol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.TELEGRAMMA);
                        break;
                    case "vol":
                        n = n.Where(a => a.Operations.operationType == (int)operationType.VOL);
                        break;
                };
            }

            //VALIDO
            if (valid != "")
            {
                if (valid == "0")
                    n = n.Where(a => a.valid == false);
                else
                    n = n.Where(a => a.valid == true);
            }

            //NOTIFICATO
            if (notificato != null)
            {
                if (notificato == true)
                    n = n.Where(a => a.notificato == notificato);

                if (notificato == false)
                    n = n.Where(a => a.notificato != true);

            }

            //CON E SENZA CODICE
            if (noCod != null)
            {
                if (noCod == "0")
                    n = n.Where(a => a.codice == null || a.codice == "");

                if (noCod == "1")
                    n = n.Where(a => a.codice != null && a.codice != "");

            }

            //DATA
            if (startDate != "")
            {
                var s = Convert.ToDateTime(startDate);
                n = n.Where(a => a.Operations.date >= s);
            }

            if (endDate != "")
            {
                var e = Convert.ToDateTime(endDate).AddDays(1);
                n = n.Where(a => a.Operations.date < e);
            }

            if (formato != "")
            {
                bool f = true;
                if (formato == "0")
                    f = false;
                n = n.Where(a => a.Operations.formatoSpeciale == f);
            }

            if (currentState != "")
            {
                int f = Convert.ToInt32(currentState);
                n = n.Where(a => a.currentState == f);
            }


            if (userId == 0 && startDate == "" && endDate == "" && sendType == "" && formato == "" && currentState == "")
            {
                var dd = DateTime.Now.ToString();
                var ss = Convert.ToDateTime(dd);
                var es = Convert.ToDateTime(dd).AddDays(1);
                n = n.Where(a => a.Operations.date >= ss).Where(a => a.Operations.date < es);
            }

            var c = n.Count();
            if (c > 0)
            {
                foreach (var t in n)
                {
                    try
                    {
                        var form = "STANDARD";
                        if (t.Operations.formatoSpeciale != null)
                            if ((bool)t.Operations.formatoSpeciale)
                                form = "SPECIALE";

                        var b = t.Operations.Users.guidUser;
                        var r = _context.Users.Where(a => a.guidUser == b).FirstOrDefault(a => a.parentId == 0);
                        var d = new DisplayInvii()
                        {
                            insertDate = t.Operations.date,
                            businessName = r.businessName != "" ? r.businessName : r.name + " " + r.lastName,
                            namesComplete = t.businessName + " " + t.name + " " + t.surname + " " + t.complementNames + ", " + t.dug + " " + t.address + " " + t.houseNumber + " " + t.complementAddress + "" + t.city + "(" + t.province + "), " + t.state,
                            valid = t.valid ? "SI" : "NO",
                            currentState = Enum.GetName(typeof(currentState), t.currentState),
                            dataConsegna = t.consegnatoDate.ToString(),
                            stato = t.stato,
                            options = "Stampa: " + ((bool)t.fronteRetro ? "SOLO Fronte" : "Fronte Retro") + "<br>" + "Tipo Stampa: " + ((bool)t.tipoStampa ? "Bianco e nero" : "Colori") + "<br>Ricevuta di ritorno: " + ((bool)t.ricevutaRitorno ? "SI" : "NO"),
                            id = t.id,
                            type = Enum.GetName(typeof(operationType), t.Operations.operationType),
                            formato = form,
                            codice = t.codice,
                            notificato = (t.notificato == true ? true : false),
                            fileName = t.fileName,
                            csv = t.Operations.csvFileName,
                            requestId = t.requestId
                        };
                        l.Add(d);
                    }
                    catch (Exception e)
                    {

                    }
                }
            }

            return l;
        }

        [HttpGet]
        [Route("Users")]
        public List<CompleteUser> GetUsers(int propertyId = 0, bool? abilitato = null, bool? test = null)
        {

            var n = _context.Users
                .Where(a => a.parentId == 0);

            if (propertyId > 0)
                n = n.Where(a => a.Property.id == propertyId);

            if (abilitato != null)
                n = n.Where(a => a.abilitato == abilitato);

            if (test != null)
                n = n.Where(a => a.areaTestUser == test);

            var l = new List<CompleteUser>();

            foreach (var u in n)
            {
                var c = new CompleteUser()
                {
                    subUsers = _context.Users.Where(a => a.parentId == u.id).Count(),
                    property = Mapper.Map<Property, PropertyDto>(u.Property),
                    user = Mapper.Map<Users, UsersDto>(u)
                };
                l.Add(c);
            }

            return l;
        }

        [HttpGet]
        [Route("Errors")]
        public Errors GetErrors(int propertyId = 0)
        {
            DateTime firstDayOfMonth = Convert.ToDateTime(DateTime.Now.Year + "/" + DateTime.Now.Month + "/01");
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);

            DateTime firstDayOfPreviousMonth = firstDayOfMonth.AddMonths(-1);
            DateTime lastDayOfPreviousMonth = firstDayOfPreviousMonth.AddMonths(1);

            DateTime today = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime tomorrow = today.AddDays(1);

            DateTime monday = Convert.ToDateTime(DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).AddDays(1).ToString("yyyy-MM-dd"));
            DateTime sunday = Convert.ToDateTime(DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek).AddDays(7).ToString("yyyy-MM-dd")).AddDays(1);

            var n = _context.Names
                .Where(a => a.currentState > 0)
                .Where(a => a.notificato != true);

            if (propertyId > 0)
                n = n.Where(a => a.Operations.Users.Property.id == propertyId);

            var month = n
                .Where(a => a.Operations.date >= firstDayOfMonth)
                .Where(a => a.Operations.date < lastDayOfMonth);

            var previousMonth = n
                .Where(a => a.Operations.date >= firstDayOfPreviousMonth)
                .Where(a => a.Operations.date < lastDayOfPreviousMonth);

            if (n.Count() == 0)
                return new Errors();

            var week = n
                .Where(a => a.Operations.date >= monday)
                .Where(a => a.Operations.date < sunday);

            var day = n
                .Where(a => a.Operations.date >= today)
                .Where(a => a.Operations.date < tomorrow);


            var e = new Errors()
            {
                previousMonthSends = previousMonth.Count(),
                previousMonthErrors = previousMonth
                .Where(a => a.currentState != (int)currentState.PresoInCarico)
                .Where(a => a.currentState != (int)currentState.InLavorazione)
                .Where(a => a.currentState != (int)currentState.AccettatoOnline)
                .Where(a => a.currentState != (int)currentState.Valorizzato)
                .Count(),
                monthSends = month.Count(),
                monthErrors = month
                .Where(a => a.currentState != (int)currentState.PresoInCarico)
                .Where(a => a.currentState != (int)currentState.InLavorazione)
                .Where(a => a.currentState != (int)currentState.AccettatoOnline)
                .Where(a => a.currentState != (int)currentState.Valorizzato)
                .Count(),
                daySends = day.Count(),
                dayErrors = day
                .Where(a => a.currentState != (int)currentState.PresoInCarico)
                .Where(a => a.currentState != (int)currentState.InLavorazione)
                .Where(a => a.currentState != (int)currentState.AccettatoOnline)
                .Where(a => a.currentState != (int)currentState.Valorizzato)
                .Count(),
                weekSends = week.Count(),
                weekErrors = week
                .Where(a => a.currentState != (int)currentState.PresoInCarico)
                .Where(a => a.currentState != (int)currentState.InLavorazione)
                .Where(a => a.currentState != (int)currentState.AccettatoOnline)
                .Where(a => a.currentState != (int)currentState.Valorizzato)
                .Count(),

            };

            return e;
        }

        [HttpGet]
        [Route("CountPartners")]
        public CountPartners CountPartners(int propertyId, int d = 0)
        {
            var date = DateTime.Now;
            var dateY = DateTime.Now;
            if (d > 0)
                dateY = Convert.ToDateTime("01/01/" + d);

            DateTime firstDayOfYear = Convert.ToDateTime(dateY.Year + "/01/01");
            DateTime lastDayOfYear = firstDayOfYear.AddYears(1).AddDays(-1);

            DateTime firstDayOfMonth = Convert.ToDateTime(date.Year + "/" + date.Month + "/01");
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            DateTime today = Convert.ToDateTime(date.ToString("yyyy-MM-dd"));
            DateTime tomorrow = today.AddDays(1);

            DateTime monday = Convert.ToDateTime(date.AddDays(-(int)DateTime.Now.DayOfWeek).AddDays(1).ToString("yyyy-MM-dd"));
            DateTime sunday = Convert.ToDateTime(date.AddDays(-(int)DateTime.Now.DayOfWeek).AddDays(7).ToString("yyyy-MM-dd")).AddDays(1);

            var n = _context.Names
                .Where(a => a.currentState > 0)
                .Where(a => a.notificato != true);

            var totalYearSend = n
                .Where(a => a.Operations.date >= firstDayOfYear)
                .Where(a => a.Operations.date < lastDayOfYear);


            if (propertyId > 0)
                n = n.Where(a => a.Operations.Users.Property.id == propertyId);


            var month = n
                .Where(a => a.Operations.date >= firstDayOfMonth)
                .Where(a => a.Operations.date < lastDayOfMonth);

            if (n.Count() == 0)
                return new CountPartners();

            var week = n
                .Where(a => a.Operations.date >= monday)
                .Where(a => a.Operations.date < sunday);

            var day = n
                .Where(a => a.Operations.date >= today)
                .Where(a => a.Operations.date < tomorrow);


            var e = new CountPartners()
            {
                monthSends = month.Count(),
                daySends = day.Count(),
                weekSends = week.Count(),
                totalYearSends = 0,
                totalYearPartnerSends = 0,
                fatturatoEasysender = 0,
                fatturatoPartner = 0
            };

            return e;
        }

        [HttpGet]
        [Route("CountPartnersNew")]
        public CountPartners CountPartnersNew(int propertyId, int d = 0)
        {
            var date = DateTime.Now;
            var dateY = DateTime.Now;
            if (d > 0)
                dateY = Convert.ToDateTime("01/01/" + d);

            DateTime firstDayOfYear = Convert.ToDateTime(dateY.Year + "/01/01");
            DateTime lastDayOfYear = firstDayOfYear.AddYears(1).AddDays(-1);

            var j = _context.GetSale(firstDayOfYear, lastDayOfYear, propertyId).FirstOrDefault();

            var e = new CountPartners()
            {
                monthSends = 0,
                daySends = 0,
                weekSends = 0,
                totalYearSends = (int)j.totalYearSends,
                totalYearPartnerSends = (int)j.totalYearPartnerSends,
                fatturatoEasysender = (decimal)j.fatturatoEasysender,
                fatturatoPartner = (decimal)j.fatturatoPartner
            };

            return e;
        }

        [HttpGet]
        [Route("FatturatoTuttiIMesi")]
        public FatturatoTuttiIMesi FatturatoTuttiIMesi(int propertyId, int d = 0)
        {
            var date = DateTime.Now;
            if (d > 0)
                date = Convert.ToDateTime("01/01/" + d);

            DateTime firstDayOfYear = Convert.ToDateTime(date.Year + "/01/01");
            DateTime lastDayOfYear = firstDayOfYear.AddYears(1).AddDays(-1);

            var n = _context.Names
               .Where(a => a.currentState > 0)
               .Where(a => a.notificato != true);

            if (n.Count() == 0)
                return new FatturatoTuttiIMesi();

            var FatturatoTuttiIMesiEasysender = new Dictionary<int, decimal>();
            var FatturatoTuttiIMesiPartner = new Dictionary<int, decimal>();

            for (var i = 0; i < 12; i++)
            {
                DateTime firstDayOfMonth = Convert.ToDateTime(date.Year + "/" + date.AddMonths(i).Month + "/01");
                DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var month = n
                    .Where(a => a.Operations.date >= firstDayOfMonth)
                    .Where(a => a.Operations.date < lastDayOfMonth);

                var monthPartner = month.Where(a => a.Operations.Users.Property.id == propertyId);

                FatturatoTuttiIMesiEasysender.Add(i, month.Sum(a => a.totalPrice) == null ? 0 : (decimal)month.Sum(a => a.totalPrice));
                FatturatoTuttiIMesiPartner.Add(i, monthPartner.Sum(a => a.totalPrice) == null ? 0 : (decimal)monthPartner.Sum(a => a.totalPrice));

            }

            var e = new FatturatoTuttiIMesi()
            {
                FatturatoTuttiIMesiEasysender = FatturatoTuttiIMesiEasysender,
                FatturatoTuttiIMesiPartner = FatturatoTuttiIMesiPartner
            };

            return e;
        }

        [HttpGet]
        [Route("FatturatoTuttiIMesiNew")]
        public FatturatoTuttiIMesi FatturatoTuttiIMesiNew(int propertyId, int d = 0)
        {
            var date = DateTime.Now;
            if (d > 0)
                date = Convert.ToDateTime("01/01/" + d);

            DateTime firstDayOfYear = Convert.ToDateTime(date.Year + "/01/01");
            DateTime lastDayOfYear = firstDayOfYear.AddYears(1).AddDays(-1);

            var j = _context.GetGrapiquePartner(firstDayOfYear, lastDayOfYear, propertyId).ToList();

            var a = new Dictionary<int, decimal>();
            var b = new Dictionary<int, decimal>();
            int y = 0;
            int x = 0;
            foreach (var i in j)
            {
                y++;
                if (y == 1)
                    a.Add((int)i.Mese - 1, (decimal)i.PriceTotal);
                else
                {
                    b.Add((int)i.Mese - 1, (decimal)i.PriceTotal);
                    y = 0;
                }
            }

            var e = new FatturatoTuttiIMesi()
            {
                FatturatoTuttiIMesiEasysender = b,
                FatturatoTuttiIMesiPartner = a
            };

            return e;
        }

        [HttpGet]
        [Route("DeleteShipping")]
        public bool DeleteShipping(int nameId)
        {

            var n = _context.Names
                .FirstOrDefault(a => a.id == nameId);

            if (n == null)
                return false;

            var c = _context.Names.Where(a => a.operationId == n.operationId).Count();
            if (c > 1)
                //ELIMINA SOLO NAME
                NamesController.DeleteItem(n.id);
            else
                //ELIMINA INTERA OPERAZIONE
                OperationsController.DeleteItem(n.operationId);

            return true;
        }

        [HttpGet]
        [Route("NotificaNames")]
        public bool NotificaNames(int nameId)
        {

            var n = _context.Names
                .FirstOrDefault(a => a.id == nameId);

            if (n == null)
                return false;

            n.notificato = true;

            NamesController.UpdateItem(nameId, Mapper.Map<Names, NamesDtos>(n));
            return true;
        }

        [HttpPost]
        [Route("ModAndSend")]
        public async Task<bool> ModAndSendAsync(ModAndSendNames ms)
        {

            var n = _context.Names.SingleOrDefault(a => a.id == ms.id);
            if (n == null)
                return false;

            n.valid = true;
            n.currentState = 0;
            n.businessName = ms.businessName;
            n.name = ms.name;
            n.surname = ms.surname;
            n.address = ms.address;
            n.dug = "";
            n.houseNumber = "";
            n.cap = ms.cap;
            n.city = ms.city;
            n.province = ms.province;
            n.state = ms.state;
            n.stato = "";
            n.locked = false;
            n.reSendGuid = null;
            n.complementAddress = ms.complementAddress;
            n.complementNames = ms.complementNames;
            n.fronteRetro = ms.fr;
            n.tipoStampa = ms.bn;
            n.ricevutaRitorno = ms.rr;
            n.tipoLettera = ms.tipoLettera;
            n.notificato = false;

            _context.SaveChanges();


            //SENDER
            var ss = Mapper.Map<SenderDto, SenderDtos>(ms.sender);
            ss.operationId = n.operationId;
            ss.userId = n.Operations.userId;
            SenderController.UpdateItem(ms.sender.id, ss);

            //SENDER AR
            if (ms.rr)
            {
                var sARs = Mapper.Map<SenderDto, SenderDtos>(ms.senderAR);
                sARs.operationId = n.operationId;
                sARs.userId = n.Operations.userId;
                SenderController.UpdateItem(ms.senderAR.id, sARs);
            }

            return true;
        }

        [HttpPost]
        [Route("AssegnaCodiceMOLCOL")]
        public async Task<string> AssegnaCodiceMOLCOL(string nId)
        {
            var namesId = nId.Split(',');
            foreach (var n in namesId)
            {
                int id = Convert.ToInt32(n);
                var names = _context.Names.FirstOrDefault(a => a.id == id);
                if (names == null)
                    continue;

                var o = _context.Operations.FirstOrDefault(a => a.id == names.operationId);
                if (o == null)
                    continue;

                if (o.operationType == (int)operationType.COL)
                {
                    var c = new COLController();
                    await c.SetState(names.id);
                }

                if (o.operationType == (int)operationType.MOL)
                {
                    var c = new MOLController();
                    await c.SetState(names.id);

                }
            }

            return "ok";
        }

        [Route("GetNamesComplete")]
        [HttpGet]
        [SwaggerResponse(200, "GetNamesComplete", typeof(IHttpActionResult))]
        public GetNamesComplete GetItemById(int id)
        {
            var x = _context.Names.SingleOrDefault(c => c.id == id);
            if (x == null)
                return null;

            var g = new GetNamesComplete()
            {
                sender = Mapper.Map<Senders, SenderDto>(x.Operations.Senders.FirstOrDefault(a => a.AR != true)),
                senderAR = Mapper.Map<Senders, SenderDto>(x.Operations.Senders.FirstOrDefault(a => a.AR == true)),
                name = Mapper.Map<Names, NamesDtos>(x),
                operation = Mapper.Map<Operations, OperationsDto>(x.Operations)
            };

            return g;
        }

        [Route("SignSend")]
        [HttpGet]
        [SwaggerResponse(200, "SignSend", typeof(IHttpActionResult))]
        public bool SignSend(int id)
        {
            var x = _context.Names.SingleOrDefault(c => c.id == id);
            if (x == null)
                return false;

            x.currentState = 1;
            x.valid = true;
            x.stato = "Presa in carico poste";

            _context.SaveChanges();

            return true;
        }


        [Route("BulkSignSend")]
        [HttpGet]
        [SwaggerResponse(200, "BulkSignSend", typeof(IHttpActionResult))]
        public bool BulkSignSend(string ids)
        {
            var namesId = ids.Split(',');
            foreach (var n in namesId)
            {
                int id = Convert.ToInt32(n);
                SignSend(id);
            }
            return true;
        }


        [Route("GetFile")]
        [HttpGet]
        public string GetFile(int nameId)
        {
            var g = "";
            var n = _context.Names.SingleOrDefault(a => a.id == nameId);
            if (n.pathRecoveryFile != null)
                return n.pathRecoveryFile;

            switch (n.Operations.operationType)
            {
                case (int)operationType.MOL:
                    var m = new MOLController();
                    g = m.RequestDCS(n.Operations.Users.guidUser, n.id);
                    break;
                case (int)operationType.COL:
                    var c = new COLController();
                    g = c.RequestDCS(n.Operations.Users.guidUser, n.id);
                    break;
                case (int)operationType.ROL:
                    var r = new RolController();
                    g = r.RequestDCS(n.Operations.Users.guidUser, n.id);
                    break;
                case (int)operationType.LOL:
                    var l = new LolController();
                    g = l.RequestDCS(n.Operations.Users.guidUser, n.id);
                    break;
                case (int)operationType.TELEGRAMMA:
                    break;
            }
            return g;
        }

    }
}
