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

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Operations")]
    [ApiExplorerSettings(IgnoreApi = true)]
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

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetOperations>))]
        public IEnumerable<GetOperations> GetList(Guid guidUser, bool complete = true, DateTime? startDate = null, DateTime? endDate = null, int? operationType = null)
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
            var x = _context.Operations.Where(c => ids.Contains(c.userId)).Where(c => c.complete == complete);
            if (x == null)
                return null;

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

            List<GetOperations> lgo = new List<GetOperations>();
            foreach (var op in x)
            {

                SenderDto so = new SenderDto();
                NamesDtos no = new NamesDtos();
                var s = _context.Senders.SingleOrDefault(a => a.operationId == op.id);
                var r = _context.Names.Where(a => a.operationId == op.id);
                GetOperations go = new GetOperations();
                go.operationId = op.id;
                go.operationDate = op.date;
                go.operationType = Enum.GetName(typeof(operationType), op.operationType);
                go.sender = Mapper.Map(s, so);
                go.recipients = Mapper.Map<List<Names>, List<NamesDtos>>(r.ToList());
                lgo.Add(go);
            }

            return lgo.OrderByDescending(a => a.operationId);
        }

        [Route("Old")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetOperations>))]
        public IEnumerable<GetOperations> GetListOld(Guid guidUser, bool complete = true, DateTime? startDate = null, DateTime? endDate = null, int? operationType = null)
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
            var x = _context.Operations.Where(c => ids.Contains(c.userId)).Where(c => c.complete == complete);
            if (x == null)
                return null;

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

            List<GetOperations> lgo = new List<GetOperations>();
            foreach (var op in x)
            {

                decimal price = 0;
                decimal vatPrice = 0;
                decimal totalPrice = 0;

                SenderDto so = new SenderDto();
                NamesDtos no = new NamesDtos();
                var s = _context.Senders.SingleOrDefault(a => a.operationId == op.id);
                var r = _context.Names.Where(a => a.operationId == op.id);
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


                List<NamesDtos> recipients = new List<NamesDtos>();
                foreach (var rec in r)
                {
                    recipients.Add(Mapper.Map(rec, no));

                    price += (decimal)rec.price;
                    vatPrice += (decimal)rec.vatPrice;
                    totalPrice += (decimal)rec.totalPrice;
                }

                go.recipients = recipients;
                go.price = price;
                go.vatPrice = vatPrice;
                go.totalPrice = totalPrice;
                lgo.Add(go);
            }

            return lgo.OrderByDescending(a => a.operationId);
        }

        [Route("GetAllOperations")]
        [SwaggerResponse(200, "GetAllOperations", typeof(IEnumerable<GetOperations>))]
        public IEnumerable<GetOperations> GetItem(Guid guidUser, string dataDa = "", string dataA = "", string username = "", string code = "", string esito = "")
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

            var lgo = new List<GetOperations>();
            foreach(var op in operations)
            {
                decimal price = 0;
                decimal vatPrice = 0;
                decimal totalPrice = 0;

                List<NamesDtos> recipients = new List<NamesDtos>();
                var r = _context.Names.Where(a => a.operationId == op.id);

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

                //ALMENO UN ELEMENTO TROVATO
                if (recipients.Count() > 0)
                {
                    SenderDto so = new SenderDto();
                    NamesDtos no = new NamesDtos();
                    var s = _context.Senders.SingleOrDefault(a => a.operationId == op.id);
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
            var s = _context.Senders.SingleOrDefault(a => a.operationId == op.id);
            var r = _context.Names.Where(a => a.operationId == op.id);
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

            var n = Mapper.Map<OperationsDto, Operations>(e);

            _context.Operations.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        private void DeleteItem(int id)
        {
            var InDb = _context.Operations.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Operations.Remove(InDb);
            _context.SaveChanges();

        }

        //[Route("RequestOperationStatus")]
        //[HttpGet]
        //public List<GetStatoRichiesta> RequestOperationStatus(Guid guidUser, int operationId)
        //{
        //    var g = new List<GetStatoRichiesta>();
        //    var o = _context.Operations.SingleOrDefault(a => a.id == operationId);
        //    switch (o.operationType)
        //    {
        //        case (int)operationType.ROL:
        //            var r = new RolController();
        //            g = r.RequestOperationStatus(guidUser, operationId);
        //            break;
        //        case (int)operationType.LOL:
        //            var l = new LolController();
        //            g = l.RequestOperationStatus(guidUser, operationId);
        //            break;
        //        case (int)operationType.TELEGRAMMA:
        //            var t = new LolController();
        //            g = t.RequestOperationStatus(guidUser, operationId);
        //            break;
        //    }

        //    return g;
        //}
    }
}
