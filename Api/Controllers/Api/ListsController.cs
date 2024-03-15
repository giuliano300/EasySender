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
using Api.DataModel;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Lists")]
    public class ListsController : ApiController
    {
        private Entities _context;

        public ListsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<GetLists>))]
        public IEnumerable<GetLists> GetList(Guid guidUser, bool? temporary = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            int[] ids = new int[u.Count()];
            int i = 0;
            foreach(var usr in u)
            {
                ids[i] = usr.id;
                i++;
            }

            var x = _context.Lists.Where(c => ids.Contains(c.userId));
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

            if (temporary != null)
                x = x.Where(a => a.temporary == temporary);
            else
                x = x.Where(a => a.temporary == false);

            NamesListsDto no = new NamesListsDto();
            List<GetLists> gls = new List<GetLists>();
            foreach (var l in x)
            {
                var r = _context.NamesLists.Where(a => a.listId == l.id);

                GetLists gl = new GetLists();
                gl.id = l.id;
                gl.name = l.name;
                gl.description = l.description;
                gl.date = l.date;

                List<NamesListsDto> recipients = new List<NamesListsDto>();
                foreach(var rec in r)
                {
                    recipients.Add(Mapper.Map(rec, no));
                }
                gl.recipients = recipients;
                gls.Add(gl);
            }

            return gls.OrderByDescending(a=>a.id);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(GetLists))]
        public GetLists GetItem(int id, Guid guidUser)
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

            var l = _context.Lists.Where(a => ids.Contains(a.Users.id)).SingleOrDefault(c => c.id == id);
            if (l == null)
                return null;

            var r = _context.NamesLists.Where(a => a.listId == l.id).ToList();

            GetLists gl = new GetLists();
            gl.id = l.id;
            gl.name = l.name;
            gl.description = l.description;
            gl.date = l.date;

            List<NamesListsDto> recipients = new List<NamesListsDto>();
            foreach (var rec in r)
            {
                var no = new NamesListsDto()
                {
                    id = rec.id,
                    businessName = rec.businessName == null ? "" : rec.businessName,
                    name = rec.name == null ? "" : rec.name,
                    surname = rec.surname == null ? "" : rec.surname,
                    dug = rec.dug == null ? "" : rec.dug,
                    address = rec.address == null ? "" : rec.address,
                    houseNumber = rec.houseNumber == null ? "" : rec.houseNumber,
                    cap = rec.cap == null ? "" : rec.cap,
                    city = rec.city == null ? "" : rec.city,
                    province = rec.province == null ? "" : rec.province,
                    state = rec.state == null ? "" : rec.state,
                    listId = rec.listId,
                    complementNames = rec.complementNames == null ? "" : rec.complementNames,
                    complementAddress = rec.complementAddress == null ? "" : rec.complementAddress,
                    fileName = rec.fileName,
                    fiscalCode = rec.fiscalCode == null ? "" : rec.fiscalCode,
                    mobile = rec.mobile == null ? "" : rec.mobile,
                    NREA = rec.NREA,
                    tipoDocumento = rec.tipoDocumento,
                    codiceDocumento = rec.codiceDocumento,
                    product = rec.product,
                    shipmentDate = rec.shipmentDate,
                    weight = rec.weight,
                    height = rec.height,
                    length = rec.length,
                    width = rec.width,
                    contentText = rec.contentText,
                    pathUrl = rec.pathUrl,
                    additionalServices = rec.additionalServices,
                    senderFromContract = rec.senderFromContract,
                    contrassegno = rec.contrassegno,
                    ritornoAlMittente = rec.ritornoAlMittente,
                    assicurazione = rec.assicurazione,
                    sms = rec.sms,
                    testoSms  = rec.testoSms,
                    logo = rec.logo,
                    pec = rec.pec,
                    AvvisoRicevimentoDigitale  = rec.AvvisoRicevimentoDigitale,
                    cron = rec.cron
                };
            recipients.Add(no);
            }
            gl.recipients = recipients;

            return gl;
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(ListsDto e)
        {
            //PARENT ID
            var u = _context.Users.SingleOrDefault(x => x.id == e.userId);
            e.userParentId = u.parentId;

            var n = Mapper.Map<ListsDto, Lists>(e);

            _context.Lists.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpGet]
        [Route("Delete")]
        public IHttpActionResult DeleteItem(int id)
        {
            var InDb = _context.Lists.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Lists.Remove(InDb);
            _context.SaveChanges();

            return Ok(id);
        }

    }
}
