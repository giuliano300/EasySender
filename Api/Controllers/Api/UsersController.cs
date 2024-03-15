using Api.DataModel;
using Api.Dtos;
using Api.Models;
using AutoMapper;
using LinqKit;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using static Api.Controllers.Api.LogsController;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Users")]
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

        [HttpPost]
        [Route("LoginNew")]
        [SwaggerResponse(200, "Login", typeof(UsersDto))]
        public GetUserAndPermissions LoginCustomerNew(login l)
        {
            var g = new GetUserAndPermissions();
            var c = new UsersDto();
            var x = _context.Users
                .Where(a => a.email == l.email)
                .SingleOrDefault(a => a.pwd == l.pwd);

            if (x == null)
                return g;
            bool all = false;

            var p = _context.UserPermits.Where(a => a.userId == x.id);
            var p0 = new UserPermits();
            if (p.Count() > 0)
                p0 = p.ToList()[0];
            else
            {
                all = true;
                p0 = null;
            }

            g.user = Mapper.Map<Users, UsersDto>(x);
            g.permits = Mapper.Map<UserPermits, UserPermitsDto>(p0);
            g.all = all;

            var lc = new LogsController();
            var lg = new LogsDto()
            {
                userId = x.id
            };

            lc.CreateItem(lg);

            return g;
        }


        [Route("{id}")]
        [SwaggerResponse(200, "Item", typeof(IHttpActionResult))]
        public IHttpActionResult GetItem(int id)
        {
            var x = _context.Users.SingleOrDefault(c => c.id == id);
            if (x == null)
                return NotFound();

            var y = Mapper.Map<Users, UsersDto>(x);

            return Ok(y);
        }

        [HttpGet]
        [Route("Recovery")]
        [SwaggerResponse(200, "Recovery", typeof(UsersDto))]
        public UsersDto Recovery(string email)
        {
            var c = new UsersDto();
            var x = _context.Users.Where(a => a.email == email);
            if (x == null)
                return null;
            if (x.Count() == 1)
            {
                c = Mapper.Map<Users, UsersDto>(x.FirstOrDefault());
                return c;
            }
            return null;
        }

        [HttpGet]
        [Route("LogOut")]
        [SwaggerResponse(200, "LogOut", typeof(UsersDto))]
        public void LogOut(int userId)
        {
            var c = new UsersDto();
            var x = _context.Users.FirstOrDefault(a => a.id == userId);

            var lc = new LogsController();
            var lg = new LogsDto()
            {
                userId = x.id,
                logType = (int)LogType.logout
            };

            lc.CreateItem(lg);
        }


        [HttpPost]
        [Route("Update/{id}")]
        public void UpdateItem(int id, UsersDto e)
        {
            var InDb = _context.Users.SingleOrDefault(c => c.id == id);
            if (InDb == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            e.changePwd = (bool)InDb.changePwd;

            Mapper.Map(e, InDb);

            _context.SaveChanges();

            if (InDb.parentId == 0)
            {
                //AGGIORNA I CAMPI AI FIGLI
                var children = _context.Users.Where(a => a.parentId == InDb.id).ToList();
                foreach (var child in children)
                {
                    child.mol = InDb.mol;
                    child.CodiceContrattoMOL = InDb.CodiceContrattoMOL;
                    child.CodiceContrattoCOL = InDb.CodiceContrattoCOL;
                    child.col = InDb.col;
                    child.logoPA = InDb.logoPA;
                    child.denominazioneEntePA = InDb.denominazioneEntePA;
                    child.infoEntePA = InDb.infoEntePA;
                    child.settoreEntePA = InDb.settoreEntePA;
                    child.codiceFiscaleEntePA = InDb.codiceFiscaleEntePA;
                    child.attivoPA = InDb.attivoPA;
                    child.downloadFile = InDb.downloadFile;
                    child.piva = InDb.piva;
                    child.usernameGED = InDb.usernameGED;
                    child.passwordGED = InDb.passwordGED;
                    child.GED = InDb.GED;
                    child.porpertyId = InDb.porpertyId;
                    child.userPriority = InDb.userPriority;
                    child.conciliazioneBollettini = InDb.conciliazioneBollettini;
                    child.abilitato = InDb.abilitato;
                    child.pacchi = InDb.pacchi;
                    child.usernamePacchi = InDb.usernamePacchi;
                    child.passwordPacchi = InDb.passwordPacchi;
                    child.centerCostPacchi = InDb.centerCostPacchi;
                    child.senderPkgId = InDb.senderPkgId;
                    child.porpertyId = InDb.porpertyId;
                  
                }

                _context.SaveChanges();
            }

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

        [HttpGet]
        [Route("ChangePwd")]
        [SwaggerResponse(200, "ChangePwd", typeof(bool))]
        public bool ChangePwd(string pwd, int userId)
        {
            var c = new Users();
            var x = _context.Users.SingleOrDefault(a => a.id == userId);
            if (x == null)
                return false;

            x.pwd = pwd;
            x.abilitato = true;
            x.changePwd = true;

            _context.SaveChanges();

            return true;
        }

        [HttpPost]
        [Route("New")]
        public int CreateItem(UsersDto e)
        {
            if (!ModelState.IsValid)
                return 0;

            var n = Mapper.Map<UsersDto, Users>(e);

            if (n.collegatoMasterId != null)
                n.changePwd = true;

            _context.Users.Add(n);
            _context.SaveChanges();

            e.id = n.id;

            return e.id;
        }


        [HttpPost]
        [Route("LoginMaster")]
        [SwaggerResponse(200, "LoginMaster", typeof(GeMasterAndChild))]
        public GeMasterAndChild LoginMaster(login l)
        {
            var g = new GeMasterAndChild();
            var c = new UsersDto();
            var x = _context.Users
                .Where(a => a.email == l.email)
                .SingleOrDefault(a => a.pwd == l.pwd);

            if (x == null)
                return null;

            var p = _context.Users.Where(a => a.collegatoMasterId == x.id).ToList();

            g.user = Mapper.Map<Users, UsersDto>(x);
            g.child = Mapper.Map<List<Users>, List<UsersDto>>(p);

            return g;
        }

        [HttpGet]
        [Route("userCollegatoMasterId")]
        [SwaggerResponse(200, "userCollegatoMasterId", typeof(UsersDto))]
        public List<UsersDto> userCollegatoMasterId(int id)
        {
            var p = _context.Users.Where(a => a.collegatoMasterId == id).ToList();

            return Mapper.Map<List<Users>, List<UsersDto>>(p);
        }



        [HttpGet]
        [Route("GetUserByUsername")]
        [SwaggerResponse(200, "GetUserByUsername", typeof(UsersDto))]
        public GetUserAndPermissions GetUserByUsername(string username, string prefix)
        {
            var g = new GetUserAndPermissions();

            var p = prefix.Split(';');

            var x = _context.Users
                 .Where(a => a.sso == true)
                 .Where(a => a.abilitato == true);

            var u = new UsersDto();

            foreach (string pf in p)
            {
                var email = username + "@" + pf;

                var y = x.FirstOrDefault(a => a.email == email);

                if (y == null)
                    continue;

                u = Mapper.Map<Users, UsersDto>(y);

            }

            if (u.id == 0)
                return null;


            bool all = true;

            g.user = u;
            g.permits = null;
            g.all = all;

            return g;
        }

    }
}
