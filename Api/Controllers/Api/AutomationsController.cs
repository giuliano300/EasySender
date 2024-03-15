using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Automations")]
    public class AutomationsController : ApiController
    {
        private Entities _context;

        public AutomationsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("RequestShipmentStatus")]
        [HttpGet]
        public GetStatoRichiesta RequestShipmentStatus(string codice, Guid guid)
        {
            GetStatoRichiesta gsr = new GetStatoRichiesta();

            var _n = _context.Names
                .Where(a => a.Operations.Users.guidUser == guid)
                .FirstOrDefault(a => a.codice == codice);

            if (_n == null)
            {
                gsr.statoDescrizione = "Spedizione non trovata";
                return gsr;
            }

            switch (_n.Operations.operationType)
            {
                case (int)operationType.ROL:
                    var rc = new RolController();
                    gsr = rc.GetStatusInviiIdRichiesta(guid, _n.requestId, codice);
                    break;

                case (int)operationType.MOL:
                    var mc = new MOLController();
                    gsr =  mc.GetStatusInviiIdRichiesta(guid, _n.requestId, codice);
                    break;

                case (int)operationType.COL:
                    var cc = new COLController();
                    gsr = cc.GetStatusInviiIdRichiesta(guid, _n.requestId, codice);
                    break;

            }

            _n.stato = gsr.statoDescrizione;
            if (gsr.finale)
            {
                _n.finalState = true;
                _n.consegnatoDate = Convert.ToDateTime(gsr.dataEsito);
                _context.SaveChanges();
            }

            return gsr;
        }

    }
}
