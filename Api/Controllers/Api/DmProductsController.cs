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

    [RoutePrefix("api/DmProducts")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DmProductsController : ApiController
    {
        private Entities _context;

        public DmProductsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<DmProductsDto>))]
        public IEnumerable<DmProductsDto> GetList()
        {
            var x = _context.DmProducts.Where(a => a.active == true);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<DmProducts, DmProductsDto>);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.DmProducts.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<DmProducts, DmProductsDto>(x));
        }

    }
}
