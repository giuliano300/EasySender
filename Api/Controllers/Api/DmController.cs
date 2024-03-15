using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Http;
using Api.Models;
using Api.ServiceLol;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Api.Dtos;
using Swashbuckle.Swagger.Annotations;
using AutoMapper;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Dm")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DmController : ApiController
    {
        private Entities _context;

        public DmController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetDm>))]
        public IEnumerable<GetDm> GetList(Guid guidUser, DateTime? startDate = null, DateTime? endDate = null)
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


            var x = _context.Dm.Where(c => ids.Contains(c.userId));
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

            NamesDmDto no = new NamesDmDto();
            List<GetDm> lgo = new List<GetDm>();
            foreach (var dm in x)
            {
                GetDm gdm = new GetDm();
                gdm.dmId = dm.id;
                gdm.dmDate = dm.date;
                gdm.productId = dm.productId;
                gdm.product = Enum.GetName(typeof(products), dm.productId);
                gdm.haveCreativity = dm.haveCreativity;
                gdm.recipientsTypeId = dm.recipientsType;
                gdm.recipientsType = Enum.GetName(typeof(recipientsType), dm.recipientsType);
                gdm.numberOfNames = dm.numberOfNames;
                gdm.netPrice = dm.netPrice;
                gdm.vatPrice = dm.vatPrice;
                gdm.totalPrice = dm.totalPrice;
                if (dm.paymentMethod != null)
                    gdm.paymentMethod = (int)dm.paymentMethod;
                gdm.paid = dm.paid;
                gdm.complete = dm.complete;


                switch (dm.recipientsType)
                {
                    case (int)recipientsType.existentList:
                        gdm.recipientsTypePublicName = "Lista Esiststente";
                        break;

                    case (int)recipientsType.newList:
                        gdm.recipientsTypePublicName = "Nuova Lista";
                        break;

                    case (int)recipientsType.requestList:
                        gdm.recipientsTypePublicName = "Lista richiesta";
                        break;
                }

                var r = _context.NamesDm.Where(a => a.dmId == dm.id);
                List<NamesDmDto> recipients = new List<NamesDmDto>();
                foreach (var rec in r)
                {
                    recipients.Add(Mapper.Map(rec, no));
                }
                gdm.recipients = recipients;

                lgo.Add(gdm);
            }

            return lgo.OrderByDescending(a => a.dmId);
        }

        [Route("All")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetDmUsers>))]
        public IEnumerable<GetDmUsers> GetAll(DateTime? startDate = null, DateTime? endDate = null)
        {

            var x = _context.Dm.Where(c => c.id > 0);
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

            NamesDmDto no = new NamesDmDto();
            List<GetDmUsers> lgo = new List<GetDmUsers>();
            foreach (var dm in x)
            {
                var u = _context.Users.SingleOrDefault(a => a.id == dm.userId);


                GetDmUsers gdm = new GetDmUsers();
                gdm.dmId = dm.id;
                gdm.dmDate = dm.date;
                gdm.productId = dm.productId;
                gdm.product = Enum.GetName(typeof(products), dm.productId);
                gdm.haveCreativity = dm.haveCreativity;
                gdm.recipientsTypeId = dm.recipientsType;
                gdm.recipientsType = Enum.GetName(typeof(recipientsType), dm.recipientsType);
                gdm.numberOfNames = dm.numberOfNames;
                gdm.netPrice = dm.netPrice;
                gdm.vatPrice = dm.vatPrice;
                gdm.totalPrice = dm.totalPrice;
                if (dm.paymentMethod != null)
                    gdm.paymentMethod = (int)dm.paymentMethod;
                gdm.paid = dm.paid;
                gdm.complete = dm.complete;

                gdm.user = Mapper.Map<Users, UsersDto>(u);

                switch (dm.recipientsType)
                {
                    case (int)recipientsType.existentList:
                        gdm.recipientsTypePublicName = "Lista Esiststente";
                        break;

                    case (int)recipientsType.newList:
                        gdm.recipientsTypePublicName = "Nuova Lista";
                        break;

                    case (int)recipientsType.requestList:
                        gdm.recipientsTypePublicName = "Lista richiesta";
                        break;
                }

                var r = _context.NamesDm.Where(a => a.dmId == dm.id);
                List<NamesDmDto> recipients = new List<NamesDmDto>();
                foreach (var rec in r)
                {
                    recipients.Add(Mapper.Map(rec, no));
                }
                gdm.recipients = recipients;

                lgo.Add(gdm);
            }

            return lgo;
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(GetDm))]
        public GetDm GetItem(int id, Guid guidUser)
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

            var dm = _context.Dm.Where(a => ids.Contains(a.Users.id)).SingleOrDefault(c => c.id == id);
            if (dm == null)
                return null;

            NamesDmDto no = new NamesDmDto();
            GetDm gdm = new GetDm();
            gdm.dmId = dm.id;
            gdm.dmDate = dm.date;
            gdm.productId = dm.productId;
            gdm.product = Enum.GetName(typeof(products), dm.productId);
            gdm.numberOfNames = dm.numberOfNames;
            gdm.haveCreativity = dm.haveCreativity;
            gdm.recipientsTypeId = dm.recipientsType;
            gdm.recipientsType = Enum.GetName(typeof(recipientsType), dm.recipientsType);

            var r = _context.NamesDm.Where(a => a.dmId == dm.id);
            List<NamesDmDto> recipients = new List<NamesDmDto>();
            foreach (var rec in r)
            {
                recipients.Add(Mapper.Map(rec, no));
            }
            gdm.recipients = recipients;

            return gdm;
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(DmDto e)
        {
            _context.Dm.RemoveRange(_context.Dm.Where(x => x.complete == false).Where(x => x.userId == e.userId));
            _context.SaveChanges();

            //PARENT ID
            var u = _context.Users.SingleOrDefault(x => x.id == e.userId);
            e.userParentId = u.parentId;

            var n = Mapper.Map<DmDto, Dm>(e);
            _context.Dm.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpGet]
        [Route("Complete")]
        public bool Complete(int id, int paymentMethod, decimal totale)
        {
            var d = _context.Dm.SingleOrDefault(c => c.id == id);
            if (d == null)
                return false;
            decimal vat = GlobalClass.GetVat(totale);

            d.complete = true;
            d.paymentMethod = paymentMethod;
            d.totalPrice = totale;
            d.vatPrice = vat;
            d.netPrice = totale - vat;
            _context.SaveChanges();

            return true;
        }

        [HttpGet]
        [Route("SignPaid")]
        public bool SignPaid(int id)
        {
            var d = _context.Dm.SingleOrDefault(c => c.id == id);
            if (d == null)
                return false;

            d.paid = true;
            _context.SaveChanges();

            return true;
        }

        [HttpGet]
        [Route("UnSignPaid")]
        public bool UnSignPaid(int id)
        {
            var d = _context.Dm.SingleOrDefault(c => c.id == id);
            if (d == null)
                return false;

            d.paid = false;
            _context.SaveChanges();

            return true;
        }

        private void DeleteItem(int id)
        {
            var InDb = _context.Dm.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Dm.Remove(InDb);
            _context.SaveChanges();

        }

    }
}
