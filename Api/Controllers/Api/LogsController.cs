using Api.Dtos;
using Api.Models;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Logs")]
    public class LogsController : ApiController
    {

        private Entities _context;

        public enum LogType
        {
            login = 1,
            logout,
            visRol,
            visLol,
            visTol,
            visVis,
            visCol,
            visMol,
            visPgk,
            sendRol,
            sendLol,
            sendTol,
            sendVis,
            sendCol,
            sendMol,
            sendPgk,
            requestReport,
            requestArchive,
            modPersonalData,
            modPwd,
            crudSender,
            crudUsers,
            visLoghi,
            addLogo,
            visErrors
        }

        public LogsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("")]
        [SwaggerResponse(200, "List", typeof(IEnumerable<LogsComplete>))]
        public IEnumerable<LogsComplete> GetList(int userId)
        {
            var xc = new List<LogsComplete>();
            var x = _context.Logs.Where(a => a.userId == userId);
            if (x == null)
                return null;

            foreach(var xx in x)
            {
                var nc = new LogsComplete()
                {
                    log = Mapper.Map<Logs, LogsDto>(xx),
                    user = Mapper.Map<Users, UsersDto>(xx.Users),
                    logType = Enum.GetName(typeof(LogType), xx.logType)
            };

                xc.Add(nc);
            }

            return xc;
        }

        [HttpPost]
        [Route("New")]
        public long CreateItem(LogsDto e)
        {
            if (!ModelState.IsValid)
                return 0;

            var n = Mapper.Map<LogsDto, Logs>(e);

            _context.Logs.Add(n);
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
