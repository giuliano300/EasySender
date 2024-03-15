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

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Pacchi")]
    public class PacchiController : ApiController
    {
        private static Entities _context;

        public PacchiController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("CheckAllFiles")]
        [HttpPost]
        public async Task<GetNumberOfCheckedNames> CheckAllFiles([FromUri] Guid guidUser, [FromBody] ObjectSubmit senderRecipients, [FromUri] int userId)
        {
            SenderDto sender = new SenderDto();
            sender = senderRecipients.sender;
            List<GetRecipent> GetRecipents = senderRecipients.recipients;

            GetNumberOfCheckedNames ncn = new GetNumberOfCheckedNames();
            List<GetCheckedNames> lgcn = new List<GetCheckedNames>();

            //MULTIPLE USERS
            var users = _context.Users.Where(a => a.guidUser == guidUser);

            //ERRORE GUID
            if (users.Count() == 0)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Utente non riconosiuto";
                return ncn;
            }

            //UTENTE INSERITORE
            var u = new Users();
            if (userId > 0)
                u = users.SingleOrDefault(a => a.id == userId);
            else
                u = users.SingleOrDefault(a => a.parentId == 0);


            //ERRORE MITTENTE
            ControlloMittente ctrlM = GlobalClass.verificaMittente(sender);
            if (!ctrlM.Valido)
            {
                ncn.numberOfValidNames = 0;
                ncn.state = "Mittente non valido";
                return ncn;
            }

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.PACCHI;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = false;
            op.formatoSpeciale = false;
            int operationId = OperationsController.CreateItem(op);


            SenderDtos ss = Mapper.Map<SenderDto, SenderDtos>(sender);
            ss.operationId = operationId;
            ss.AR = false;
            int senderId = SenderController.CreateItem(ss);


            int validNames = 0;
            foreach (var GetRecipent in GetRecipents.ToList())
            {
                int id = (int)GetRecipent.recipient.id;

                NamesDtos nos = Mapper.Map<NamesDto, NamesDtos>(GetRecipent.recipient);
                nos.operationId = operationId;
                nos.requestId = null;
                nos.guidUser = null;
                nos.valid = true;

                nos.fronteRetro = false;
                nos.ricevutaRitorno = false;
                nos.tipoStampa = false;


                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

                //NUMERO DI PAGINE
                nos.numberOfPages = 0;

                var nc = new NamesController();
                int idName = nc.CreateItem(nos, u.userPriority);
                validNames++;

                GetCheckedNames gcn = new GetCheckedNames()
                {
                    name = nos,
                    valid = true,
                    price = new Prices()
                };

                lgcn.Add(gcn);

            }

            ncn.numberOfValidNames = validNames;
            ncn.checkedNames = lgcn;
            ncn.state = "Inserimento valido!";
            ncn.valid = true;
            ncn.operationId = operationId;

            return ncn;

        }

    }
}
