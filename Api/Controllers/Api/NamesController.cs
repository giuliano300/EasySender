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

    [RoutePrefix("api/Names")]
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

        [HttpGet]
        [Route("ErroriNotificati")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NamesDtos>))]
        public IEnumerable<NamesDtos> ErroriNotificati(int userId)
        {
            //MULTIPLE USERS
            var u = _context.Users.FirstOrDefault(a => a.id == userId);
            if (u == null)
                return null;

            var x = _context.Names.Where(a => a.notificato == true);

            if (u.parentId == 0)
                x = x.Where(a => a.Operations.userId == userId || a.Operations.userParentId == userId);
            else
                x = x.Where(a => a.Operations.userId == userId);

            if (x == null)
                return null;

            return x.OrderByDescending(a => a.id).ToList().Select(Mapper.Map<Names, NamesDtos>);
        }

        [HttpGet]
        [Route("NotificheNonLette")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NamesDtos>))]
        public int NotificheNonLette(int userId)
        {
            //MULTIPLE USERS
            var u = _context.Users.FirstOrDefault(a => a.id == userId);
            if (u == null)
                return 0;

            var x = _context.Names
                .Where(a => a.notificato == true)
                .Where(a => a.notificaLetta != true);

            if (u.parentId == 0)
                x = x.Where(a => a.Operations.userId == userId || a.Operations.userParentId == userId);
            else
                x = x.Where(a => a.Operations.userId == userId);

            if (x == null)
                return 0;

            return x.Count();
        }

        [HttpGet]
        [Route("LeggiNotificheNonLette")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NamesDtos>))]
        public void LeggiNotificheNonLette(int userId)
        {
            //MULTIPLE USERS
            var u = _context.Users.FirstOrDefault(a => a.id == userId);
            if (u == null)
                return;

            var x = _context.Names.Where(a => a.notificato == true);

            if (u.parentId == 0)
                x = x.Where(a => a.Operations.userId == userId || a.Operations.userParentId == userId);
            else
                x = x.Where(a => a.Operations.userId == userId);

            if (x == null)
                return;

            foreach (var r in x.ToList())
                r.notificaLetta = true;

            _context.SaveChanges();
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

        [Route("ItemById")]
        [HttpGet]
        [SwaggerResponse(200, "ItemById", typeof(IHttpActionResult))]
        public IHttpActionResult GetItemById(int id)
        {
            var x = _context.Names.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<Names, NamesDtos>(x));
        }

        public int CreateItem(NamesDtos e, int priority)
        {
            try {
                var n = Mapper.Map<NamesDtos, Names>(e);
                n.locked = false;
                n.namePriority = priority;
                _context.Names.Add(n);
                _context.SaveChanges();

                e.id = n.id;
            }
            catch (Exception  ex)
            {
                var q = ex;
            };

            return e.id;
        }

        public static void UpdateItem(int id, NamesDtos e)
        {
            Entities _context = new Entities();
            var InDb = _context.Names.Where(a => a.locked == false).SingleOrDefault(c => c.id == id);
            if (InDb == null)
                return;

            Mapper.Map(e, InDb);

            _context.SaveChanges();

        }

        public static void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.Names.Where(a => a.locked == false).SingleOrDefault(c => c.id == id);
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


        [Route("GetFile")]
        [HttpGet]
        public string GetFile(int nameId, Guid guidUser)
        {
            var g = "";
            var n = _context.Names.SingleOrDefault(a => a.id == nameId);
            if (n.pathRecoveryFile != null)
                return n.pathRecoveryFile;

            switch (n.Operations.operationType)
            {
                case (int)operationType.MOL:
                    var m = new MOLController();
                    g = m.RequestDCS(guidUser, n.id);
                    break;
                case (int)operationType.COL:
                    var c = new COLController();
                    g = c.RequestDCS(guidUser, n.id);
                    break;
                case (int)operationType.ROL:
                    var r = new RolController();
                    g = r.RequestDCS(guidUser, n.id);
                    break;
                case (int)operationType.LOL:
                    var l = new LolController();
                    g = l.RequestDCS(guidUser, n.id);
                    break;
                case (int)operationType.TELEGRAMMA:
                    break;
            }
            return g;
        }

        [Route("GetFileGED")]
        [HttpGet]
        public string GetFileGED(int nameId, Guid guidUser)
        {
            var g = "";
            var n = _context.Names.SingleOrDefault(a => a.id == nameId);
            if (n.pathGEDUrl != null)
                return n.pathGEDUrl;

            return g;
        }
    }
}
