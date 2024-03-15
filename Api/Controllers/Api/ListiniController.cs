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

    [RoutePrefix("api/Listini")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ListiniController : ApiController
    {
        private Entities _context;

        public ListiniController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpGet]
        [Route("")]
        public Preventivo GetListini(int productId, int qta)
        {
            var l = _context.Listini.SingleOrDefault(a => a.productId == productId);

            decimal creativita = l.creativita;
            decimal lista = qta * l.liste;
            decimal distribuzione = qta * l.distrubuzione;
            decimal stampa = 0;

            if (qta >= 0 && qta < 1000)
                stampa = l.stampa500 * qta;

            if (qta >= 1000 && qta < 2000)
                stampa = l.stampa1000 * qta;

            if (qta >= 2000 && qta < 3000)
                stampa = l.stampa2000 * qta;

            if (qta >= 3000 && qta < 4000)
                stampa = l.stampa3000 * qta;

            if (qta >= 4000 && qta < 5000)
                stampa = l.stampa4000 * qta;

            if (qta == 5000)
                stampa = l.stampa5000 * qta;

            decimal totale = creativita + lista + distribuzione + stampa;

            var p = new Preventivo()
            {
                creativita = creativita,
                stampa = stampa,
                lista = lista,
                distribuzione = distribuzione,
                totale = totale
            };

            return p;
        }

    }
}
