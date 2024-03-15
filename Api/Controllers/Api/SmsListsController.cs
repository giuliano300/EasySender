using Api.Dtos;
using Api.Models;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/SmsLists")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SmsListsController : ApiController
    {
            private Entities _context;

            public SmsListsController()
            {
                _context = new Entities();
            }

            protected override void Dispose(bool disposing)
            {
                _context.Dispose();
            }

            [HttpGet]
            [Route("")]
            [SwaggerResponse(200, "List", typeof(IEnumerable<SmsListsDto>))]
            public IEnumerable<SmsListsDto> GetList()
            {
                var x = _context.SmsLists.Where(a => a.id > 0);
                if (x == null)
                    return null;

                return x.ToList().Select(Mapper.Map<SmsLists, SmsListsDto>);
            }

            [Route("{id}")]
            [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
            public IHttpActionResult GetItem(int id)
            {
                var x = _context.SmsLists.SingleOrDefault(c => c.id == id);
                if (x == null)
                    return NotFound();

                return Ok(Mapper.Map<SmsLists, SmsListsDto>(x));
            }

            [Route("Price/{qta}")]
            [SwaggerResponse(200, "Item", typeof(decimal))]
            public decimal GetPrice(int qta)
            {
                var x = _context.SmsLists.Where(c => c.valueMin <= qta).Where(c => c.valueMax > qta).SingleOrDefault();
                if (x == null)
                    return 0;

                return x.price;
            }

    }
}
