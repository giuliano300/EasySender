using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using Api.ServiceLol;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Api.Dtos;
using Swashbuckle.Swagger.Annotations;
using AutoMapper;
using Api.DataModel;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Operations")]
    public class OperationsController : ApiController
    {
        private Entities _context;

        public OperationsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetOperations>))]
        public IEnumerable<GetOperations> GetList(Guid guidUser, bool complete = true, DateTime? startDate = null,
            DateTime? endDate = null, int? operationType = null, int userId = 0, bool completeTransmission = false, string top = "")
        {
            var o = new List<GetOperations>();
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return o;

            var nids = u.Select(a => a.id).ToList();

            var x = _context.Operations.Where(c => nids.Contains(c.userId)).Where(c => c.complete == complete);
            if (x.Count() == 0)
                return o;


            if (userId > 0)
                x = x.Where(a => a.userId == userId);

            if (startDate != null)
            {
                DateTime td = (DateTime)startDate;
                x = x.Where(a => a.date >= td);
            }

            if (endDate != null)
            {
                DateTime ed = (DateTime)endDate;
                ed = ed.AddDays(1);
                x = x.Where(a => a.date < ed);
            }

            if (operationType != null)
            {
                int op = (int)operationType;
                x = x.Where(a => a.operationType == op);
            }
            else
            {
                x = x.Where(a => a.operationType != 6);
            }

            //NOT COMPLETE TRANSMISSION
            if (!completeTransmission)
                x = x.Where(y => y.Names.Any(a => a.requestId == null && a.valid == true));
            else
                x = x.Where(y => y.Names.All(a => a.requestId != null || a.valid == false));

            if (top != "")
                x = x.OrderByDescending(a => a.id).Take(Convert.ToInt32(top));


            List<GetOperations> lgo = new List<GetOperations>();
            foreach (var op in x)
            {
                var r = _context.Names.Where(a => a.operationId == op.id);
                var total = r.Where(a => a.totalPrice != null).Sum(a => a.totalPrice);
                SenderDto so = new SenderDto();
                NamesDtos no = new NamesDtos();
                var s = _context.Senders.Where(a => a.AR != true).FirstOrDefault(a => a.operationId == op.id);
                GetOperations go = new GetOperations();
                go.operationId = op.id;
                go.type = op.operationType;
                go.formato = op.formatoSpeciale != true ? "Standard" : "Speciale";
                go.operationDate = op.date;
                go.operationType = Enum.GetName(typeof(operationType), op.operationType);
                go.totalPrice = total != null ? Convert.ToDecimal(total) : 0;
                go.sender = Mapper.Map(s, so);
                go.recipients = Mapper.Map<List<Names>, List<NamesDtos>>(r.ToList());
                lgo.Add(go);
            }

            return lgo.OrderByDescending(a => a.operationId);
        }

        [HttpGet]
        [Route("Total")]
        [SwaggerResponse(200, "List", typeof(TotalOperations))]
        public TotalOperations Total(Guid guidUser, bool complete = true, DateTime? startDate = null,
       DateTime? endDate = null, int? operationType = null, int userId = 0, bool completeTransmission = false, int pageSize = 50, int start = 0, string reciverName = "")
        {
            var t = new TotalOperations();
            var o = new List<GetOperations>();
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return t;

            var nids = u.Select(a => a.id).ToList();

            var x = _context.Operations.Where(c => nids.Contains(c.userId)).Where(c => c.complete == complete);
            if (x.Count() == 0)
                return t;


            if (userId > 0)
                x = x.Where(a => a.userId == userId);

            if (startDate != null)
            {
                DateTime td = (DateTime)startDate;
                x = x.Where(a => a.date >= td);
            }

            if (endDate != null)
            {
                DateTime ed = (DateTime)endDate;
                ed = ed.AddDays(1);
                x = x.Where(a => a.date < ed);
            }

            if (operationType != null)
            {
                int op = (int)operationType;
                x = x.Where(a => a.operationType == op);
            }
            else
            {
                x = x.Where(a => a.operationType != 6);
            }

            //NOT COMPLETE TRANSMISSION
            if (!completeTransmission)
                x = x.Where(y => y.Names.Any(a => a.requestId == null && a.valid == true));
            else
                x = x.Where(y => y.Names.All(a => a.requestId != null || a.valid == false));

            if (reciverName != "")
                x = x.Where(a => a.Names.Any(y => y.businessName.Contains(reciverName) || y.name.Contains(reciverName) || y.surname.Contains(reciverName)));

            int count = x.Count();

            if (pageSize > 0)
                x = x.OrderByDescending(a => a.id).Skip(start * pageSize).Take(pageSize);


            List<GetOperations> lgo = new List<GetOperations>();
            foreach (var op in x)
            {
                var r = _context.Names.Where(a => a.operationId == op.id);
                var total = r.Where(a => a.totalPrice != null).Sum(a => a.totalPrice);
                SenderDto so = new SenderDto();
                NamesDtos no = new NamesDtos();
                var s = _context.Senders.Where(a => a.AR != true).FirstOrDefault(a => a.operationId == op.id);
                GetOperations go = new GetOperations();
                go.operationId = op.id;
                go.type = op.operationType;
                go.formato = op.formatoSpeciale != true ? "Standard" : "Speciale";
                go.operationDate = op.date;
                go.operationType = Enum.GetName(typeof(operationType), op.operationType);
                go.totalPrice = total != null ? Convert.ToDecimal(total) : 0;
                go.sender = Mapper.Map(s, so);
                go.recipients = Mapper.Map<List<Names>, List<NamesDtos>>(r.ToList());
                lgo.Add(go);
            }

            t.count = count;
            t.getOperations = lgo;

            return t;
        }

        [Route("GetAllOperations")]
        [SwaggerResponse(200, "GetAllOperations", typeof(IEnumerable<GetOperations>))]
        public IEnumerable<GetOperations> GetItem(Guid guidUser, string dataDa = "", string dataA = "", string username = "",
            string code = "", string esito = "", int prodotto = 0, int userId = 0, string pp = "", string bollettini = "", string pagato = "")
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


            var operations = _context.Operations.Where(a => ids.Contains(a.Users.id)).Where(c => c.complete == true);
            if (operations == null)
                return null;

            if (userId > 0)
                operations = operations.Where(a => a.userId == userId || a.userParentId == userId);

            if (dataDa != "")
            {
                DateTime da = Convert.ToDateTime(dataDa);
                operations = operations.Where(a => a.date >= da);
            }

            if (dataA != "")
            {
                DateTime a = Convert.ToDateTime(dataA).AddDays(1);
                operations = operations.Where(x => x.date < a);
            }

            if (prodotto > 0)
            {
                operations = operations.Where(x => x.operationType == prodotto);
            }
            else
            {
                operations = operations.Where(x => x.operationType != (int)operationType.VOL);
            }

            if (pp != "")
            {
                int px = Convert.ToInt32(pp);
                operations = operations.Where(x => x.Names.Any(y => y.tipoDocumento == px));
            }



            var lgo = new List<GetOperations>();
            foreach (var op in operations)
            {
                decimal price = 0;
                decimal vatPrice = 0;
                decimal totalPrice = 0;

                List<NamesDtos> recipients = new List<NamesDtos>();
                var r = _context.Names.Where(a => a.operationId == op.id).Where(a => a.requestId != null);

                foreach (var rec in r)
                {
                    var n = new NamesDtos();
                    bool trovatoUsername = true;
                    bool trovatoCode = true;
                    bool trovatoEsito = true;

                    if (username == "" && code == "" && esito == "" && bollettini == "")
                        recipients.Add(Mapper.Map<Names, NamesDtos>(rec));
                    else
                    {
                        if (username != "")
                            if (!rec.name.Contains(username) && !rec.surname.Contains(username) && !rec.businessName.Contains(username))
                                trovatoUsername = false;

                        if (code != "")
                        {
                            if (rec.codice != code && !rec.fiscalCode.Contains(code))
                                trovatoCode = false;
                        }
                        if (esito != "")
                        {
                            bool e = Convert.ToBoolean(esito);
                            if (!rec.valid == e)
                                trovatoEsito = false;
                        }

                        var b = _context.Bulletins.SingleOrDefault(a => a.namesId == rec.id);
                        //BOLLETTINI
                        if (bollettini != "")
                        {
                            var bul = Convert.ToBoolean(bollettini);
                            //SE E' RICHIESTO IL BOLLETTINO
                            if (bul && b == null)
                                continue;

                            //SE NON E' RICHIESTO IL BOLLETTINO
                            if (!bul && b != null)
                                continue;

                            //SE NON ESITE IL BOLLETTINO
                            if (pagato != "" && b == null)
                                continue;

                            if (pagato != "" && bul)
                            {
                                //SE SI CERCANO I BOLLETITNI PAGATI
                                var pay = Convert.ToBoolean(pagato);
                                if (pay && b.Pagato != true)
                                    continue;

                                //SE SI CERCANO I BOLLETITNI NON PAGATI
                                if (!pay && b.Pagato == true)
                                    continue;
                            }

                        }


                        if (!trovatoUsername || !trovatoCode)
                            if (b != null)
                            {
                                var t = username;
                                if (t == "")
                                    t = code;

                                if (t != "")
                                    if (b.CodiceCliente.Contains(t))
                                    {
                                        trovatoUsername = true;
                                        trovatoCode = true;
                                    }
                            }


                        if (trovatoUsername && trovatoCode && trovatoEsito)
                            recipients.Add(Mapper.Map<Names, NamesDtos>(rec));
                    }

                    price += (decimal)rec.price;
                    vatPrice += (decimal)rec.vatPrice;
                    totalPrice += (decimal)rec.totalPrice;
                }

                //ALMENO UN ELEMENTO TROVATO
                if (recipients.Count() > 0)
                {
                    SenderDto so = new SenderDto();
                    NamesDtos no = new NamesDtos();
                    var s = _context.Senders.Where(a => a.AR != true).FirstOrDefault(a => a.operationId == op.id);
                    GetOperations go = new GetOperations();
                    go.operationId = op.id;
                    go.operationDate = op.date;
                    go.operationType = Enum.GetName(typeof(operationType), op.operationType);
                    go.sender = Mapper.Map(s, so);


                    List<operationFeaturesDto> opfeatsDto = new List<operationFeaturesDto>();
                    var f = _context.operationFeatures.Where(c => c.operationId == op.id);
                    foreach (var feat in f)
                    {
                        operationFeaturesDto opfeatDto = new operationFeaturesDto();
                        opfeatDto.featureType = feat.featureType;
                        opfeatDto.featureValue = feat.featureValue;
                        opfeatsDto.Add(opfeatDto);
                    }
                    go.operationFeatures = opfeatsDto;


                    go.recipients = recipients;
                    go.price = price;
                    go.vatPrice = vatPrice;
                    go.totalPrice = totalPrice;

                    lgo.Add(go);

                }

            }

            return lgo;
        }


        [Route("GetAllOperationsNewNoBulletins")]
        [SwaggerResponse(200, "GetAllOperationsNewNoBulletins", typeof(IEnumerable<GetOperationNames>))]
        public IEnumerable<GetOperationNames> GetAllOperationsNewNoBulletins(Guid guidUser, string dataDa = "", string dataA = "", string username = "",
  string code = "", string esito = "", int prodotto = 0, int userId = 0, string pp = "", string mittente = "", string ar = "", int start = 0, int pageSize = 0)
        {
            //var exclude = _context.Bulletins.Where(a => a.namesId > 0).Select(p => p.namesId);

            var lgo = new List<GetOperationNames>();
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser).ToList();
            if (u.Count() == 0)
                return null;

            var user = u.FirstOrDefault().id;
            var Hideprice = u.FirstOrDefault().hidePrice;

            var names = _context.Names
                //.Where(a => !exclude.Contains(a.id))
                .Where(a => a.valid == true)
                .Where(a => a.currentState > 0)
                .Where(a => a.Operations.complete == true)
                .Where(a => a.Operations.userId == user || a.Operations.userParentId == user);

            if (names == null)
                return null;

            if (userId > 0)
            {
                var us = _context.Users.FirstOrDefault(a => a.id == userId);
                if (us.parentId == 0)
                    names = names
                     .Where(a => a.Operations.userId == userId ||
                     a.Operations.userParentId == userId);
                else
                    names = names
                     .Where(a => a.Operations.userId == userId);
            }

            if (mittente != "")
                names = names.Where(a => a.Operations.Senders.Select(x => x.businessName).Contains(mittente));

            if (ar != "")
            {
                bool ricevutaRitorno = Convert.ToBoolean(ar);
                names = names.Where(a => a.ricevutaRitorno == ricevutaRitorno);
            }

            if (dataDa != "")
            {
                DateTime da = Convert.ToDateTime(dataDa);
                names = names.Where(a => a.Operations.date >= da);
            }

            if (dataA != "")
            {
                DateTime a = Convert.ToDateTime(dataA).AddDays(1);
                names = names.Where(x => x.Operations.date < a);
            }

            if (prodotto > 0)
                names = names.Where(x => x.Operations.operationType == prodotto);

            if (esito != "")
            {
                bool e = Convert.ToBoolean(esito);
                names = names.Where(x => x.valid == e);
            }


            if (pp != "")
            {
                int px = Convert.ToInt32(pp);
                names = names.Where(x => x.tipoDocumento == px);
            }

            if (username != "")
                names = names.Where(a => a.businessName.ToLower().StartsWith(username.ToLower()) || a.businessName.ToLower().Contains(username.ToLower()) || a.name.ToLower().Contains(username.ToLower()) || a.surname.ToLower().Contains(username.ToLower()) || a.name.ToLower().StartsWith(username.ToLower()) || a.surname.ToLower().StartsWith(username.ToLower()));

            if (code != "")
                names = names.Where(a => a.codice == code || a.fiscalCode == code);

            if (pageSize > 0)
                names = names.OrderByDescending(a => a.id).Skip(start * pageSize).Take(pageSize);

            var c = names.ToList();

            foreach (var n in names)
            {
                var recipient = new GetRecipentNew()
                {
                    bulletin = null,
                    recipient = Mapper.Map<Names, NamesDtos>(n)
                };

                var go = new GetOperationNames()
                {
                    price = (decimal)n.price,
                    vatPrice = (decimal)n.vatPrice,
                    totalPrice = (decimal)n.totalPrice,
                    operationId = n.operationId,
                    operationType = Enum.GetName(typeof(operationType), n.Operations.operationType),
                    operationDate = n.Operations.date,
                    sender = Mapper.Map<Senders, SenderDto>(n.Operations.Senders.FirstOrDefault(a => a.AR != true)),
                    recipient = recipient,
                    hidePrice = Hideprice
                };

                lgo.Add(go);

            }
            return lgo;
        }

        [Route("GetAllOperationsNewWithBulletins")]
        [SwaggerResponse(200, "GetAllOperationsNewWithBulletins", typeof(IEnumerable<GetOperationNames>))]
        public IEnumerable<GetOperationNames> GetItemsBulletins(Guid guidUser, string dataDa = "", string dataA = "", string username = "",
            string code = "", string esito = "", int prodotto = 0, int userId = 0, string pp = "", string pagato = "", string dataDaPayments = "", string dataAPayments = "", string mittente = "", string ar = "")
        {
            var lgo = new List<GetOperationNames>();
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var names = _context.Names.Join(_context.Bulletins,
                name => name.id,
                bulletin => bulletin.namesId,
                (name, bulletin) => new { Names = name, Bulletins = bulletin })
                .Where(a => a.Names.valid == true)
                .Where(a => a.Names.currentState > 0)
                .Where(a => a.Names.Operations.complete == true);

            if (names == null)
                return null;

            if (userId > 0)
            {
                var us = _context.Users.FirstOrDefault(a => a.id == userId);
                if (us.parentId == 0)
                    names = names
                     .Where(a => a.Names.Operations.userId == userId ||
                     a.Names.Operations.userParentId == userId);
                else
                    names = names
                     .Where(a => a.Names.Operations.userId == userId);
            }

            if (mittente != "")
                names = names.Where(a => a.Names.Operations.Senders.Select(x => x.businessName).Contains(mittente));

            if (ar != "")
            {
                bool ricevutaRitorno = Convert.ToBoolean(ar);
                names = names.Where(a => a.Names.ricevutaRitorno == ricevutaRitorno);
            }

            if (dataDa != "")
            {
                DateTime da = Convert.ToDateTime(dataDa);
                names = names.Where(a => a.Names.Operations.date >= da);
            }

            if (dataA != "")
            {
                DateTime a = Convert.ToDateTime(dataA).AddDays(1);
                names = names.Where(x => x.Names.Operations.date < a);
            }

            if (dataDaPayments != "")
            {
                DateTime da = Convert.ToDateTime(dataDaPayments);
                names = names.Where(a => a.Bulletins.DataPagamento >= da);
            }

            if (dataAPayments != "")
            {
                DateTime a = Convert.ToDateTime(dataAPayments).AddDays(1);
                names = names.Where(x => x.Bulletins.DataPagamento < a);
            }

            if (prodotto > 0)
                names = names.Where(x => x.Names.Operations.operationType == prodotto);

            if (esito != "")
            {
                bool e = Convert.ToBoolean(esito);
                names = names.Where(x => x.Names.valid == e);
            }


            if (pp != "")
            {
                int px = Convert.ToInt32(pp);
                names = names.Where(x => x.Names.tipoDocumento == px);
            }

            if (username != "")
                names = names.Where(a => a.Names.businessName.StartsWith(username) || a.Names.businessName.Contains(username));

            if (code != "")
                names = names
                    .Where(a => a.Names.codice == code || a.Names.fiscalCode == code || a.Bulletins.CodiceCliente == code || a.Bulletins.CodiceCliente.StartsWith(code));


            if (pagato != "")
            {
                var pay = Convert.ToBoolean(pagato);
                if (pay)
                    names = names.Where(a => a.Bulletins.Pagato == pay);
                else
                    names = names.Where(a => a.Bulletins.Pagato != true);
            }


            foreach (var n in names)
            {

                var recipient = new GetRecipentNew()
                {
                    bulletin = Mapper.Map<Bulletins, BulletinsDtos>(n.Bulletins),
                    recipient = Mapper.Map<Names, NamesDtos>(n.Names)
                };

                var go = new GetOperationNames()
                {
                    price = (decimal)n.Names.price,
                    vatPrice = (decimal)n.Names.vatPrice,
                    totalPrice = (decimal)n.Names.totalPrice,
                    operationId = n.Names.operationId,
                    operationType = Enum.GetName(typeof(operationType), n.Names.Operations.operationType),
                    operationDate = n.Names.Operations.date,
                    sender = Mapper.Map<Senders, SenderDto>(n.Names.Operations.Senders.FirstOrDefault(a => a.AR != true)),
                    recipient = recipient
                };

                List<operationFeaturesDto> opfeatsDto = new List<operationFeaturesDto>();
                var f = _context.operationFeatures.Where(c => c.operationId == n.Names.Operations.id);
                foreach (var feat in f)
                {
                    operationFeaturesDto opfeatDto = new operationFeaturesDto();
                    opfeatDto.featureType = feat.featureType;
                    opfeatDto.featureValue = feat.featureValue;
                    opfeatsDto.Add(opfeatDto);
                }
                go.operationFeatures = opfeatsDto;


                lgo.Add(go);

            }
            return lgo;
        }



        [Route("Items/{id}")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetOperationsNew>))]
        public GetOperationsNew GetItemNew(int id, Guid guidUser, string username = "", string code = "", string esito = "")
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

            var op = _context.Operations.Where(a => ids.Contains(a.Users.id)).SingleOrDefault(c => c.id == id);
            if (op == null)
                return null;

            SenderDto so = new SenderDto();
            NamesDtos no = new NamesDtos();
            var s = _context.Senders
                .Where(a => a.AR != true)
                .FirstOrDefault(a => a.operationId == op.id);

            var r = _context.Names.Where(a => a.operationId == op.id);
            GetOperationsNew go = new GetOperationsNew();
            go.operationId = op.id;
            go.operationDate = op.date;
            go.type = op.operationType;
            go.operationType = Enum.GetName(typeof(operationType), op.operationType);
            go.sender = Mapper.Map(s, so);


            List<operationFeaturesDto> opfeatsDto = new List<operationFeaturesDto>();
            var f = _context.operationFeatures.Where(c => c.operationId == op.id);
            foreach (var feat in f)
            {
                operationFeaturesDto opfeatDto = new operationFeaturesDto();
                opfeatDto.featureType = feat.featureType;
                opfeatDto.featureValue = feat.featureValue;
                opfeatsDto.Add(opfeatDto);
            }
            go.operationFeatures = opfeatsDto;

            decimal price = 0;
            decimal vatPrice = 0;
            decimal totalPrice = 0;

            List<GetRecipentNew> recipients = new List<GetRecipentNew>();
            foreach (var rec in r)
            {
                var b = _context.Bulletins.SingleOrDefault(a => a.namesId == rec.id);
                var gr = new GetRecipentNew()
                {
                    recipient = Mapper.Map<Names, NamesDtos>(rec),
                    bulletin = Mapper.Map<Bulletins,BulletinsDtos>(b)
                };
                var n = new NamesDtos();
                bool trovatoUsername = true;
                bool trovatoCode = true;
                bool trovatoEsito = true;

                if (username == "" && code == "" && esito == "")
                    recipients.Add(gr);
                else
                {

                    if (username != "")
                        if (!rec.name.Contains(username) && !rec.surname.Contains(username) && !rec.businessName.Contains(username))
                            trovatoUsername = false;

                    if (code != "")
                        if (!rec.codice.Contains(code))
                            trovatoCode = false;

                    if (esito != "")
                    {
                        bool e = Convert.ToBoolean(esito);
                        if (!rec.valid == e)
                            trovatoEsito = false;
                    }

                    if(!trovatoUsername || !trovatoCode)
                        if (b != null)
                        {
                            var t = username;
                            if (t == "")
                                t = code;

                            if (t != "")
                                if (b.CodiceCliente.Contains(t)) {
                                    trovatoUsername = true;
                                    trovatoCode = true;
                                }
                        }

                    if (trovatoUsername && trovatoCode && trovatoEsito)
                        recipients.Add(gr);
                }

                price += (decimal)rec.price;
                vatPrice += (decimal)rec.vatPrice;
                totalPrice += (decimal)rec.totalPrice;
            }
            go.recipients = recipients;
            go.price = price;
            go.vatPrice = vatPrice;
            go.totalPrice = totalPrice;

            return go;
        }

        [Route("{id}")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetOperations>))]
        public GetOperations GetItem(int id, Guid guidUser, string username = "", string code = "", string esito = "")
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

            var op = _context.Operations.Where(a => ids.Contains(a.Users.id)).SingleOrDefault(c => c.id == id);
            if (op == null)
                return null;

            SenderDto so = new SenderDto();
            NamesDtos no = new NamesDtos();
            var s = _context.Senders
                .Where(a => a.AR != true)
                .FirstOrDefault(a => a.operationId == op.id);

            var r = _context.Names.Where(a => a.operationId == op.id);
            GetOperations go = new GetOperations();
            go.operationId = op.id;
            go.operationDate = op.date;
            go.type = op.operationType;
            go.operationType = Enum.GetName(typeof(operationType), op.operationType);
            go.sender = Mapper.Map(s, so);


            List<operationFeaturesDto> opfeatsDto = new List<operationFeaturesDto>();
            var f = _context.operationFeatures.Where(c => c.operationId == op.id);
            foreach (var feat in f)
            {
                operationFeaturesDto opfeatDto = new operationFeaturesDto();
                opfeatDto.featureType = feat.featureType;
                opfeatDto.featureValue = feat.featureValue;
                opfeatsDto.Add(opfeatDto);
            }
            go.operationFeatures = opfeatsDto;

            decimal price = 0;
            decimal vatPrice = 0;
            decimal totalPrice = 0;

            List<NamesDtos> recipients = new List<NamesDtos>();
            foreach (var rec in r)
            {

                var n = new NamesDtos();
                bool trovatoUsername = true;
                bool trovatoCode = true;
                bool trovatoEsito = true;

                if (username == "" && code == "" && esito == "")
                    recipients.Add(Mapper.Map<Names, NamesDtos>(rec));
                else
                {
                    if (username != "")
                        if (!rec.name.Contains(username) && !rec.surname.Contains(username) && !rec.businessName.Contains(username))
                            trovatoUsername = false;

                    if (code != "")
                        if (!rec.codice.Contains(code))
                            trovatoCode = false;

                    if (esito != "")
                    {
                        bool e = Convert.ToBoolean(esito);
                        if (!rec.valid == e)
                            trovatoEsito = false;
                    }

                    if (trovatoUsername && trovatoCode && trovatoEsito)
                        recipients.Add(Mapper.Map<Names, NamesDtos>(rec));
                }

                price += (decimal)rec.price;
                vatPrice += (decimal)rec.vatPrice;
                totalPrice += (decimal)rec.totalPrice;
            }
            go.recipients = recipients;
            go.price = price;
            go.vatPrice = vatPrice;
            go.totalPrice = totalPrice;

            return go;
        }

        public static int CreateItem(OperationsDto e)
        {
            Entities _context = new Entities();

            //PARENT ID
            var u = _context.Users.SingleOrDefault(x => x.id == e.userId);
            e.userParentId = u.parentId;
            e.operationPriority = (int)u.userPriority;

            var n = Mapper.Map<OperationsDto, Operations>(e);

            _context.Operations.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpPost]
        [Route("New")]
        public int NewItem(OperationsDto e)
        {
            //PARENT ID
            var u = _context.Users.SingleOrDefault(x => x.id == e.userId);
            e.userParentId = u.parentId;
            e.operationPriority = (int)u.userPriority;

            var n = Mapper.Map<OperationsDto, Operations>(e);

            _context.Operations.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        public static void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.Operations.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Operations.Remove(InDb);
            _context.SaveChanges();

        }


        [Route("SetComplete")]
        [HttpGet]
        public GetOperations SetComplete(int id, Guid guidUser)
        {
            var InDb = _context.Operations.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            InDb.complete = true;
            _context.SaveChanges();

            return GetItem(id, guidUser);

        }

        [Route("GetOperationType")]
        [HttpGet]
        public int GetOperationType(int id)
        {
            var InDb = _context.Operations.SingleOrDefault(c => c.id == id);
            return InDb.operationType;

        }
    }
}
