using Api.DataModel;
using Api.Dtos;
using Api.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Payments")]
    public class PaymentsController : ApiController
    {
        private Entities _context;

        public PaymentsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [HttpPost]
        [Route("Bulletin")]
        public List<CompleteNameBulletin> CompleteNames(List<Codice> codici, [FromUri] Guid guidUser)
        {

            var l = new List<CompleteNameBulletin>();

            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return l;

            foreach(var c in codici)
            {
                try { 
                    var b = _context.Bulletins
                        .Where(a => a.namesId > 0)
                        .FirstOrDefault(a => a.CodiceCliente.StartsWith(c.codiceCliente));

                    if (b == null)
                        continue;

                    var n = _context.Names
                        .Where(a => a.Operations.Users.guidUser == guidUser)
                        .SingleOrDefault(a => a.id == b.namesId);

                    if (n == null)
                        continue;

                    b.Pagato = true;
                    b.Controllato = true;
                    b.DataPagamento = c.dataPagamento;

                    //RIPORTO I BOLLETTINI
                    var cn = new CompleteNameBulletin()
                    {
                        bulletin = Mapper.Map<Bulletins, BulletinsDto>(b),
                        name = Mapper.Map<Names, NamesDto>(n)
                    };

                    l.Add(cn);
                }
                catch (Exception e)
                {
                    var cn = new CompleteNameBulletin()
                    {
                        bulletin  = new BulletinsDto(),
                        name = new NamesDto() { businessName = e.Message.ToString()}
                    };

                    l.Add(cn);

                }
            }

            _context.SaveChanges();



            return l;
        }




        [HttpGet]
        [Route("Bill")]
        public List<GetBill> CompleteBill([FromUri] Guid guidUser, string st, string ed)
        {
            var pp = "";
            var start = Convert.ToDateTime(st);
            var end = Convert.ToDateTime(ed);

            var names = new List<Names>();
            var res = new List<GetBill>();

            var u = _context.Users.Where(a => a.guidUser == guidUser).ToList();
            var mainUser = u[0].id;

            names = _context.Names
           .Where(a => a.currentState == 1 || a.currentState == 2)
               .Where(a => a.Operations.userId == mainUser || a.Operations.userParentId == mainUser)
               .Where(a => a.Operations.date >= start && a.Operations.date <= end)
               .ToList();

            foreach (var name in names)
            {
                var send = name.Operations.Senders.ToString();

                switch (name.operationType)
                {
                    case 1:
                        pp = "ROL";
                        break;
                    case 2:
                        pp = "LOL";
                        break;
                    case 3:
                        pp = "TOL";
                        break;
                    case 4:
                        pp = "MOL";
                        break;
                    case 5:
                        pp = "COL";
                        break;
                    case 6:
                        pp = "VOL";
                        break;
                    case 7:
                        pp = "PACCO";
                        break;
                    case 8:
                        pp = "AGOL";
                        break;
                    default:
                        break;
                }

                var gb = new GetBill()
                {
                    codice = name.codice,
                    prodotto = pp,
                    DataInserimento = name.insertDate,
                    destinatario = name.businessName + ' ' + name.name + ' ' + name.surname,
                    completamentoNomeDestinatario = name.complementNames,
                    indirizzoDestinatario = name.dug + ' ' + name.address + ' ' + name.complementAddress,
                    completamentoIndirizzoDestinatario = name.complementAddress,
                    cap = name.cap,
                    citta = name.city,
                    provincia = name.province,
                    mittente = name.Operations.Users.businessName,
                    operationId = name.operationId,
                    operatore = name.Operations.Users.name + ' ' + name.Operations.Users.lastName,
                    requestId = name.requestId,
                    stato = name.stato,

                };
                res.Add(gb);
            }

            return res;
        }

    }
}
