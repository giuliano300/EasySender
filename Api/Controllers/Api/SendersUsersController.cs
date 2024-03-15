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

    [RoutePrefix("api/SendersUsers")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SendersUsersController : ApiController
    {
        private Entities _context;

        public SendersUsersController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<SendersUsersDto>))]
        public IEnumerable<SendersUsersDto> GetList(int userId)
        {
            var x = _context.SendersUsers.Where(a => a.userId == userId);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<SendersUsers, SendersUsersDto>);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.SendersUsers.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<SendersUsers, SendersUsersDto>(x));
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(SendersUsersDto e)
        {
            //PARENT ID
            var u = _context.Users.SingleOrDefault(x => x.id == e.userId);
            e.userParentId = u.parentId;

            var n = Mapper.Map<SendersUsersDto, SendersUsers>(e);

            _context.SendersUsers.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpPost]
        [Route("Update/{id}")]
        public IHttpActionResult UpdateItem(int id, SendersUsersDto e)
        {
            var InDb = _context.SendersUsers.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

            return Ok(id);
        }

        [HttpGet]
        [Route("Delete")]
        public IHttpActionResult DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.SendersUsers.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.SendersUsers.Remove(InDb);
            _context.SaveChanges();

            return Ok(id);
        }

    }
}
