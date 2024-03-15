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

    [ApiExplorerSettings(IgnoreApi = true)]
    [RoutePrefix("api/NamesDm")]
    public class NamesDmController : ApiController
    {
        private Entities _context;

        public NamesDmController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NamesDmDto>))]
        public IEnumerable<NamesDmDto> GetList(int dmId, Guid guidUser)
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
            var x = _context.NamesDm.Where(a => ids.Contains(a.Dm.userId)).Where(a => a.dmId == dmId);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<NamesDm, NamesDmDto>);
        }

        [Route("Item")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id, Guid guidUser)
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

            var x = _context.NamesDm.Where(a => ids.Contains(a.Dm.userId)).SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<NamesDm, NamesDmDto>(x));
        }

        public static int CreateItem(NamesDmDto e)
        {
            Entities _context = new Entities();
            var n = Mapper.Map<NamesDmDto, NamesDm>(e);

            _context.NamesDm.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        public static void UpdateItem(int id, NamesDmDto e)
        {
            Entities _context = new Entities();
            var InDb = _context.NamesDm.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

        }

        public static void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.NamesDm.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.NamesDm.Remove(InDb);
            _context.SaveChanges();

        }

        [HttpGet]
        [Route("DuplicateMultiple")]
        public bool DuplicateMultiple(int listId, int dmId)
        {
            var le = _context.NamesLists.Where(a => a.listId == listId);
            foreach (var e in le.ToList())
            {
                var n = Mapper.Map<NamesLists, NamesDm>(e);
                n.dmId = dmId;
                _context.NamesDm.Add(n);
                _context.SaveChanges();

            }
            return true;
        }

    }
}
