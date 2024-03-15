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
    [RoutePrefix("api/Notifications")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NotificationsController : ApiController
    {
        private Entities _context;

        public NotificationsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NotificationsDto>))]
        public IEnumerable<NotificationsDto> GetList(int userId = 0, string enabled = "")
        {

            var u = _context.Notifications.Where(a => a.id > 0);
            if (u.Count() == 0)
                return null;

            if (enabled != "")
            {
                bool en = Convert.ToBoolean(enabled);
                u = u.Where(a => a.enabled == en);
            }

            if (userId > 0)
            {
                var usr = _context.Users.SingleOrDefault(a => a.id == userId);
                if (usr.parentId > 0)
                    userId = usr.parentId;

                 u = u.Where(a => a.usersId.Contains(userId.ToString()) || a.usersId == null);
           }

            var n = Mapper.Map<IEnumerable<Notifications>, IEnumerable<NotificationsDto>>(u);

            return n.OrderByDescending(a => a.id);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(NotificationsDto))]
        public NotificationsDto GetItem(int id)
        {

            var r = _context.Notifications.SingleOrDefault(a => a.id == id);
            if (r == null)
                return null;

            var n = Mapper.Map<Notifications, NotificationsDto>(r);

            return n;
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(NotificationsDto e)
        {
            var n = Mapper.Map<NotificationsDto, Notifications>(e);
            _context.Notifications.Add(n);
            _context.SaveChanges();
            e.id = n.id;
            return e.id;
        }

        [HttpPost]
        [Route("Update/{id}")]
        public NotificationsDto UpdateItem(int id, NotificationsDto e)
        {
            var InDb = _context.Notifications.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

            return e;

        }

        [HttpGet]
        [Route("Delete")]
        public IHttpActionResult DeleteItem(int id)
        {
            var InDb = _context.Notifications.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Notifications.Remove(InDb);
            _context.SaveChanges();

            return Ok(id);
        }

    }
}
