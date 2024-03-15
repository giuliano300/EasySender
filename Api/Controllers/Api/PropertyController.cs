using Api.DataModel;
using Api.Dtos;
using Api.Models;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Property")]
    public class PropertyController : ApiController
    {
        private Entities _context;

        public PropertyController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<PropertyDto>))]
        public IEnumerable<PropertyDto> GetList (int parentId = 0)
        {
            var x = _context.Property;
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<Property, PropertyDto>);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.Property.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<Property, PropertyDto>(x));
        }

        [HttpPost]
        [Route("Login")]
        [SwaggerResponse(200, "Login", typeof(PropertyDto))]
        public PropertyDto LoginCustomerNew(login l)
        {
            var x = _context.Property
                .Where(a => a.username == l.email)
                .SingleOrDefault(a => a.password == l.pwd);

            if (x == null)
                return null;
            return Mapper.Map<Property, PropertyDto>(x);
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(PropertyDto e)
        {
            if (!ModelState.IsValid)
                return 0;

            var n = Mapper.Map<PropertyDto, Property>(e);

            _context.Property.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpPost]
        [Route("Update/{id}")]
        public void UpdateItem(int id, PropertyDto e)
        {
            var InDb = _context.Property.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);


            Mapper.Map(e, InDb);

            _context.SaveChanges();
        }

        [HttpGet]
        [Route("Delete/{id}")]
        public void DeleteItem(int id)
        {
            var InDb = _context.Property.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Property.Remove(InDb);
            _context.SaveChanges();

        }


    }
}
