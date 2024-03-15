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

    [RoutePrefix("api/Names")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NamesController : ApiController
    {
        private Entities _context;

        public NamesController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NamesDtos>))]
        public IEnumerable<NamesDtos> GetList(int operationId, Guid guidUser)
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

            var x = _context.Names.Where(a => ids.Contains(a.Operations.userId)).Where(a => a.operationId == operationId);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<Names, NamesDtos>);
        }

        [Route("Item")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(string requestId, Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var x = _context.Names.SingleOrDefault(c => c.requestId == requestId);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<Names, NamesDtos>(x));
        }

        public int CreateItem(NamesDtos e)
        {
            var n = Mapper.Map<NamesDtos, Names>(e);
            n.locked = false;

            _context.Names.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        public static void UpdateItem(int id, NamesDtos e)
        {
            Entities _context = new Entities();
            var InDb = _context.Names.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

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

        [HttpGet]
        [Route("GetCompleteCount")]
        [SwaggerResponse(200, "GetCompleteCount", typeof(GetNamesCount))]
        public GetNamesCount GetCompleteCount(Guid guidUser, int operationId)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var x = _context.Names.Where(c => c.operationId == operationId).Where(a => a.valid == true);
            if (x == null)
                return null;

            int t = x.ToList().Count();
            int complete = x.Where(a => a.requestId != null).ToList().Count();
            int incomplete = x.Where(a => a.requestId == null).ToList().Count();
            int percentage = (t > 0 ? complete * 100 / t : 0);

            var g = new GetNamesCount()
            {
                totalNumber = t,
                complete = complete,
                incomplete = incomplete,
                percentage = percentage
            };

            return g;
        }

        [HttpGet]
        [Route("GetOrderCount")]
        [SwaggerResponse(200, "GetOrderCount", typeof(GetNamesCount))]
        public GetNamesCount GetOrderCount(Guid guidUser,int listId)
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

            var l = _context.NamesLists.Where(a => a.listId == listId).Count();

            var o = _context.Operations.Where(a => ids.Contains(a.userId)).Where(a => a.complete == false).OrderByDescending(a => a.id).ToList();
            if (o.Count() == 0)
                return null;
            int id = o[0].id;

            var x = _context.Names.Where(c => c.operationId == id).Where(a => a.requestId != null);
            var n = x.ToList();
            if (n.Count() == 0)
                return null;

            int t = l;
            int complete = x.Where(a => a.guidUser != null).ToList().Count();
            int incomplete = x.Where(a => a.valid == false).ToList().Count();

            var g = new GetNamesCount()
            {
                totalNumber = t,
                complete = complete,
                incomplete = incomplete,
                percentage = complete * 100 / t
            };

            return g;
        }

    }
}
