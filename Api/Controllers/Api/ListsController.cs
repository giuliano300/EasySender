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
    [RoutePrefix("api/Lists")]
    [ApiExplorerSettings(IgnoreApi = true)]
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
                var no = Mapper.Map<NamesLists, NamesListsDto>(rec);
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
