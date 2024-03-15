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

    [RoutePrefix("api/Sender")]
    public class SenderController : ApiController
    {
        private Entities _context;

        public SenderController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<SenderDtos>))]
        public IEnumerable<SenderDtos> GetList(int operationId)
        {
            var x = _context.Senders.Where(a => a.operationId == operationId);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<Senders, SenderDtos>);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.Senders.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<Senders, SenderDtos>(x));
        }

        [Route("New")]
        [HttpPost]
        public int New(SenderDtos e)
        {
            var n = Mapper.Map<SenderDtos, Senders>(e);

            _context.Senders.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }
        public static int CreateItem(SenderDtos e)
        {
            Entities _context = new Entities();
            var n = Mapper.Map<SenderDtos, Senders>(e);

            _context.Senders.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        public static void UpdateItem(int id, SenderDtos e)
        {
            Entities _context = new Entities();
            var InDb = _context.Senders.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            try { 
            Mapper.Map(e, InDb);

            _context.SaveChanges();
            }catch(Exception ex)
            {
                var r = ex;
            }

        }

        public static void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.Names.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Names.Remove(InDb);
            _context.SaveChanges();

        }

    }
}
