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
using System.Web.Http.Description;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Users")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UsersController : ApiController
    {
        private Entities _context;

        public UsersController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpGet]
        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<UsersDto>))]
        public IEnumerable<UsersDto> GetList (int parentId = 0)
        {
            var x = _context.Users.Where(a => a.parentId == parentId);
            if (x == null)
                return null;

            return x.ToList().Select(Mapper.Map<Users, UsersDto>);
        }

        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.Users.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            return Ok(Mapper.Map<Users, UsersDto>(x));
        }

        [HttpPost]
        [Route("Login")]
        [SwaggerResponse(200, "Login", typeof(UsersDto))]
        public UsersDto LoginCustomer(login l)
        {
            var c = new UsersDto();
            var x = _context.Users.Where(a => a.email == l.email).SingleOrDefault(a => a.pwd == l.pwd);
            if (x == null)
                return c;

            c = Mapper.Map<Users, UsersDto>(x);

            return c;
        }

        [HttpGet]
        [Route("ChangePwd")]
        [SwaggerResponse(200, "ChangePwd", typeof(bool))]
        public bool ChangePwd(string pwd, int userId)
        {
            var c = new Users();
            var x = _context.Users.SingleOrDefault(a => a.id == userId);
            if (x == null)
                return false;

            var cDto = new UsersDto();
            x.pwd = pwd;

            Mapper.Map(x, cDto);

            _context.SaveChanges();

            return true;
        }

        [HttpGet]
        [Route("ControlPwd")]
        [SwaggerResponse(200, "ControlPwd", typeof(bool))]
        public bool ControlPwd(string pwd, int userId)
        {
            var c = new Users();
            var x = _context.Users.Where(a => a.id == userId).SingleOrDefault(a => a.pwd == pwd);
            if (x == null)
                return false;

            return true;
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(UsersDto e)
        {
            if (!ModelState.IsValid)
                return 0;

            var n = Mapper.Map<UsersDto, Users>(e);

            _context.Users.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }

        [HttpPost]
        [Route("Update/{id}")]
        public void UpdateItem(int id, UsersDto e)
        {
            var InDb = _context.Users.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            Mapper.Map(e, InDb);

            _context.SaveChanges();

        }

        [HttpGet]
        [Route("Delete/{id}")]
        public void DeleteItem(int id)
        {
            var InDb = _context.Users.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            _context.Users.Remove(InDb);
            _context.SaveChanges();

        }


    }
}
