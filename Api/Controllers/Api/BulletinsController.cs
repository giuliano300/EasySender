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
    [RoutePrefix("api/Bulletins")]
    public class BulletinsController : ApiController
    {
        private Entities _context;

        public BulletinsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }


        public static int CreateItem(BulletinsDto e)
        {
            Entities _context = new Entities();
            var b = Mapper.Map<BulletinsDto, Bulletins>(e);

            _context.Bulletins.Add(b);
            _context.SaveChanges();

            e.id = b.id;

            return e.id;
        }

        public static void UpdateItem(int id, BulletinsDto e)
        {
            Entities _context = new Entities();
            var InDb = _context.Bulletins.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

        }

        public static void DeleteItem(int id)
        {
            Entities _context = new Entities();
            var InDb = _context.Bulletins.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Bulletins.Remove(InDb);
            _context.SaveChanges();

        }

        public static void UpdateItemByNamesListsId(int namesListsId, int namesId)
        {
            Entities _context = new Entities();
            var InDb = _context.Bulletins.Where(a => a.namesId == 0).SingleOrDefault(c => c.namesListsId == namesListsId);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            InDb.namesId = namesId;

            _context.SaveChanges();
        }

        public static void DeleteItemByNamesListsId(int namesListsId)
        {
            Entities _context = new Entities();
            var InDb = _context.Bulletins.SingleOrDefault(c => c.namesListsId == namesListsId);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Bulletins.Remove(InDb);
            _context.SaveChanges();

        }

    }
}
