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
    [RoutePrefix("api/TemporaryValidateTable")]
    public class TemporaryValidateTableController : ApiController
    {
        private Entities _context;

        public TemporaryValidateTableController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<TemporaryValidateTableDto>))]
        public IEnumerable<TemporaryValidateTableDto> GetList(int userId, string sessionId)
        {
            var u = _context.TemporaryValidateTable.Where(a => a.userId == userId).Where(a => a.sessionId == sessionId).ToList();
            if (u.Count() == 0)
                return null;

            var n = Mapper.Map<List<TemporaryValidateTable>, List<TemporaryValidateTableDto>>(u);

            return n;
        }

        [HttpPost]
        [Route("New")]
        public Guid CreateItem(TemporaryValidateTableDto e)
        {
            //PARENT ID
            var n = Mapper.Map<TemporaryValidateTableDto, TemporaryValidateTable>(e);

            _context.TemporaryValidateTable.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpGet]
        [Route("Delete")]
        public IHttpActionResult DeleteItem(int userId, string sessionId)
        {
            var InDb = _context.TemporaryValidateTable.Where(a => a.userId == userId).Where(a => a.sessionId == sessionId).ToList();
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            foreach (var i in InDb)
            {
                _context.TemporaryValidateTable.Remove(i);
            }

            _context.SaveChanges();

            return Ok("Deleted");
        }

        [HttpGet]
        [Route("GetPercentage")]
        public int  GetPercentage(int userId, string sessionId)
        {
            var InDb = _context.TemporaryValidateTable.Where(a => a.userId == userId).Where(a => a.sessionId == sessionId).ToList();
            int total = 0;
            decimal percentage = 0;
            int number = InDb.Count();
            if (number > 0)
            {
                total= InDb.FirstOrDefault().totalNames;
                decimal perc = Decimal.Divide(number, total);
                percentage = Math.Round(perc * 100, 0);
          }
           
            return Convert.ToInt32(percentage);
        }

    }
}
