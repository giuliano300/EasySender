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

    [RoutePrefix("api/NamesLists")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NamesListsController : ApiController
    {
        private Entities _context;

        public NamesListsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<NamesListsDto>))]
        public IEnumerable<NamesListsDto> GetList(Guid guidUser, int listId = 0)
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

            var x = _context.NamesLists.Where(a => ids.Contains(a.Lists.userId));
            if (listId > 0)
                x = x.Where(a => a.listId == listId);
            else
            {
                var l = _context.Lists.OrderByDescending(a => a.id).FirstOrDefault(a => ids.Contains(a.userId));
                x = x.Where(a => a.listId == l.id);
            }

            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<NamesLists, NamesListsDto>);
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

            var x = _context.NamesLists.Where(a => ids.Contains(a.Lists.userId)).SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<NamesLists, NamesListsDto>(x));
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(NamesListsDto e)
        {
            Entities _context = new Entities();
            var n = Mapper.Map<NamesListsDto, NamesLists>(e);

            _context.NamesLists.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpPost]
        [Route("NewMultiple")]
        public bool CreateMultpleItem(List<NamesListsDto> le)
        {
            foreach (var e in le.ToList())
            {
                var n = Mapper.Map<NamesListsDto, NamesLists>(e);
                _context.NamesLists.Add(n);
                _context.SaveChanges();

            }
            return true;
        }

        [HttpPost]
        [Route("NewMultipleWithBulletin")]
        public bool NewMultipleWithBulletin(GetRecipentLists le)
        {
            int i = 0;
            foreach (var e in le.recipient.ToList())
            {
                var n = Mapper.Map<NamesListsDto, NamesLists>(e);
                _context.NamesLists.Add(n);
                _context.SaveChanges();

                if (le.bulletin != null)
                {
                    var bdto = le.bulletin[i];
                    bdto.namesListsId = n.id;
                    var b = Mapper.Map<BulletinsDtos, Bulletins>(bdto);

                    _context.Bulletins.Add(b);
                    _context.SaveChanges();
                }
                i++;
            }
            return true;
        }

        [HttpGet]
        [Route("ReplaceLists")]
        public bool ReplaceLists(string ids, int listId)
        {
            var items = ids.Split(',');
            foreach(var i in items)
            {

                int id = Convert.ToInt32(i.Split('|')[0]);
                string fileName = i.Split('|')[1];

                var n = _context.NamesLists.SingleOrDefault(a => a.id == id);
                n.fileName = fileName;
                n.listId = listId;

                _context.NamesLists.Add(n);
                _context.SaveChanges();
            }

            return true;
        }

        [HttpPost]
        [Route("Update/{id}")]
        public void UpdateItem(int id, NamesListsDto e)
        {
            var InDb = _context.NamesLists.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

        }

        [HttpGet]
        [Route("Delete")]
        public  void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.NamesLists.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.NamesLists.Remove(InDb);
            _context.SaveChanges();

        }

    }
}
