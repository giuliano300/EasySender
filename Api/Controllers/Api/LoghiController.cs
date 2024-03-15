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

    [RoutePrefix("api/Loghi")]
    public class LoghiController : ApiController
    {
        private Entities _context;

        public LoghiController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<LoghiDto>))]
        public IEnumerable<LoghiDto> GetList(int userId, int? parentUserId = null)
        {
            var x = _context.Loghi.Where(a => a.userId == userId);
            if (x == null)
                return null;

            if(parentUserId != null)
                 x = _context.Loghi.Where(a => a.userId == userId || a.parentUserId == parentUserId);

            return x.ToList().Select(Mapper.Map<Loghi, LoghiDto>);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.Loghi.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<Loghi, LoghiDto>(x));
        }

        [HttpPost]
        [Route("New")]
        public  int CreateItem(LoghiDto e)
        {
            var n = Mapper.Map<LoghiDto, Loghi>(e);

            _context.Loghi.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpPost]
        [Route("Update/{id}")]
        public void UpdateItem(int id, LoghiDto e)
        {
            var InDb = _context.Loghi.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);
            try
            {
                Mapper.Map(e, InDb);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                var r = ex;
            }

        }

        [HttpGet]
        [Route("Delete")]
        public void DeleteItem(int id)
        {
            var InDb = _context.Loghi.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Loghi.Remove(InDb);
            _context.SaveChanges();

        }

    }
}
