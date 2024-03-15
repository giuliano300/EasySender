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

    [RoutePrefix("api/DmConfiguration")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DmConfigurationController : ApiController
    {
        private Entities _context;

        public DmConfigurationController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<DmConfigurationsDto>))]
        public IEnumerable<DmConfigurationsDto> GetList(int dmId, Guid guidUser)
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

            var x = _context.DmConfigurations.Where(a => ids.Contains(a.Dm.userId)).Where(a => a.dmId == dmId);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<DmConfigurations, DmConfigurationsDto>);
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

            var x = _context.DmConfigurations.Where(a => ids.Contains(a.Dm.userId)).SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<DmConfigurations, DmConfigurationsDto>(x));
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(DmConfigurationsDto e)
        {
            var n = Mapper.Map<DmConfigurationsDto, DmConfigurations>(e);
            _context.DmConfigurations.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        public static void UpdateItem(int id, DmConfigurationsDto e)
        {
            Entities _context = new Entities();
            var InDb = _context.DmConfigurations.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

        }

        public static void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.DmConfigurations.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.DmConfigurations.Remove(InDb);
            _context.SaveChanges();

        }

    }
}
