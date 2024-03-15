using Api.Dtos;
using Api.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Sending")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SynchronizeSendingController : ApiController
    {

        private Entities _context;

        public SynchronizeSendingController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        //[Route("Sync")]
        //[HttpGet]
        //public async Task<bool> Sync()
        //{
        //    var namesGrouped = _context.Names.Where(a => a.valid == true).Where(a => a.requestId == null).Where(a => a.Operations.demoOperation == false).Take(30).GroupBy(a => a.operationId).ToList();

        //    var list = new List<Names>();

        //    for(var i=0; i < namesGrouped.Count; i++)
        //    {
        //        var names = namesGrouped[i].ToList();
        //        foreach (var n in names)
        //        {
        //            var sender = _context.Senders.SingleOrDefault(a => a.operationId == n.Operations.id);
        //            var bulletin = _context.Bulletins.Where(a => a.namesId == n.id).ToList();
        //            Bulletins b = null;
        //            if (bulletin.Count()> 0)
        //            {
        //                b = bulletin[0];
        //            }

        //            var senderDto = Mapper.Map<Senders, SenderDto>(sender);

        //            var gr = new GetRecipent() {
        //                recipient = Mapper.Map<Names,NamesDto>(n),
        //                bulletin = Mapper.Map<Bulletins, BulletinsDtos>(b)
        //            };

        //            switch (n.Operations.operationType)
        //            {
        //                case (int)operationType.ROL:
        //                    var r = new RolController();
        //                    var newNamesRol = await r.sendNames(gr, n.operationId, senderDto, n.Operations.userId);
        //                    if(newNamesRol.valid)
        //                        list.Add(newNamesRol);
        //                    break;
        //                case (int)operationType.LOL:
        //                    var l = new LolController();
        //                    var newNamesLol = await l.sendNames(gr, n.operationId, senderDto, n.Operations.userId);
        //                    if (newNamesLol.valid)
        //                        list.Add(newNamesLol);
        //                    break;
        //            }
        //        }
        //        if(list.Count > 0)
        //            switch (names[0].Operations.operationType)
        //            {
        //                case (int)operationType.ROL:
        //                    var r = new RolController();
        //                    await r.ValorizzaConfirm(list, names[0].Operations.Users.guidUser);
        //                    break;

        //                case (int)operationType.LOL:
        //                    var l = new LolController();
        //                    await l.ValorizzaConfirm(list, names[0].Operations.Users.guidUser);
        //                    break;
        //            }

        //    }

        //    return true;
        //}

    }
}
