using Api.Dtos;
using Api.Models;
using Api.ServiceRol;
using Api.ServiceGED;
using AutoMapper;
using FluentFTP;
using GemBox.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Xml.Linq;
using Microsoft.Web.Services3.Security.Tokens;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net;
using System.Drawing;
using Api.DataModel;

namespace Api.Controllers.Api
{
    /// <summary>
    /// Raccomandata online
    /// </summary>
    [RoutePrefix("api/Agol")]
    public class AgolController : ApiController
    {

        private static Entities _context;

        public AgolController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        private void createFeatures(tipoStampa tipoStampa, fronteRetro fronteRetro, ricevutaRitorno ricevutaRitorno, int operationId)
        {
            var op1 = new operationFeatures();
            op1.operationId = operationId;
            op1.featureType = "tipo stampa";
            op1.featureValue = Enum.GetName(typeof(tipoStampa), tipoStampa);
            _context.operationFeatures.Add(op1);
            _context.SaveChanges();

            var op2 = new operationFeatures();
            op2.operationId = operationId;
            op2.featureType = "fronte Retro";
            op2.featureValue = Enum.GetName(typeof(fronteRetro), fronteRetro);
            _context.operationFeatures.Add(op2);
            _context.SaveChanges();

            var op3 = new operationFeatures();
            op3.operationId = operationId;
            op3.featureType = "ricevuta Ritorno";
            op3.featureValue = Enum.GetName(typeof(ricevutaRitorno), ricevutaRitorno);
            _context.operationFeatures.Add(op3);
            _context.SaveChanges();

        }


        [Route("CheckAllFiles")]
        [HttpPost]
        public async Task<GetNumberOfCheckedNames> CheckAllFiles([FromUri] Guid guidUser, [FromBody] ObjectSubmit senderRecipients, [FromUri] bool tsc,
            [FromUri] bool frc, [FromUri] bool rrc, [FromUri] bool frm, [FromUri] int userId)
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

            //ERRORE MITTENTE AR
            if (senderRecipients.senderAR != null)
            {
                ControlloMittente ctrlMAR = GlobalClass.verificaMittente(senderRecipients.senderAR);
                if (!ctrlMAR.Valido)
                {
                    ncn.numberOfValidNames = 0;
                    ncn.state = "Destinatario AR non valido";
                    return ncn;
                }
            }

            OperationsController oc = new OperationsController();
            OperationsDto op = new OperationsDto();
            op.date = DateTime.Now;
            op.name = " Operazione del " + DateTime.Now.ToString("dd/MM/yyyy");
            op.userId = u.id;
            op.operationType = (int)operationType.AGOL;
            op.demoOperation = u.demoUser;
            op.areaTestOperation = u.areaTestUser;
            op.complete = false;
            op.formatoSpeciale = frm;
            op.csvFileName = senderRecipients.csvFile;
            int operationId = OperationsController.CreateItem(op);

            tipoStampa ts = tipoStampa.colori;
            if (tsc)
                ts = tipoStampa.biancoNero;

            fronteRetro fr = fronteRetro.fronte;
            if (!frc)
                fr = fronteRetro.fronteRetro;

            ricevutaRitorno rr = ricevutaRitorno.si;
            if (!rrc)
                rr = ricevutaRitorno.no;


            createFeatures(ts, fr, rr, operationId);

            SenderDtos ss = Mapper.Map<SenderDto, SenderDtos>(sender);
            ss.operationId = operationId;
            ss.AR = false;
            int senderId = SenderController.CreateItem(ss);

            //CREAZIONE MITTENTE AR
            if (senderRecipients.senderAR != null)
            {
                SenderDtos ssAR = Mapper.Map<SenderDto, SenderDtos>(senderRecipients.senderAR);
                ssAR.operationId = operationId;
                ssAR.AR = true;
                int senderIdAR = SenderController.CreateItem(ssAR);
            }


            int validNames = 0;
            foreach (var GetRecipent in GetRecipents.ToList())
            {
                int id = (int)GetRecipent.recipient.id;
                var b = _context.Bulletins.Where(a => a.namesListsId == id).ToList();
                if (b.Count() > 0)
                {
                    GetRecipent.bulletin = Mapper.Map<Bulletins, BulletinsDtos>(b[0]);
                };

                NamesDtos nos = Mapper.Map<NamesDto, NamesDtos>(GetRecipent.recipient);
                nos.operationType = op.operationType;
                nos.operationId = operationId;
                nos.requestId = null;
                nos.guidUser = null;
                nos.valid = true;

                nos.fronteRetro = frc;
                nos.ricevutaRitorno = rrc;
                nos.tipoStampa = tsc;

                nos.insertDate = DateTime.Now;
                nos.currentState = (int)currentState.inAttesa;

                //NUMERO DI PAGINE
                ComponentInfo.SetLicense("ADWG-YKI0-D7LE-5JK9");
                var document = new PdfDocument();
                try 
                { 
                    document = PdfDocument.Load(nos.fileName);
                    nos.numberOfPages = document.Pages.Count();
                }
                catch(Exception e)
                {
                    var error = e.Message;
                }

                var nc = new NamesController();
                int idName = nc.CreateItem(nos, u.userPriority);
                if (GetRecipent.bulletin != null)
                {
                    BulletinsDto bos = Mapper.Map<BulletinsDtos, BulletinsDto>(GetRecipent.bulletin);
                    bos.namesId = idName;
                    BulletinsController.CreateItem(bos);
                }
                validNames++;

                GetCheckedNames gcn = new GetCheckedNames()
                {
                    name = nos,
                    valid = true,
                    price = frm ? GlobalClass.GetFilePriceSpecialFormat(operationType.ROL, document, tsc) : GlobalClass.GetFilePrice(operationType.ROL, document, tsc)
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


        private static byte[] GetLogo(string pathLogo)
        {
            byte[] imgdata = File.ReadAllBytes(pathLogo);

            return imgdata;
        }

        [Route("NewOne")]
        [HttpPost]
        public async Task<int> InsertOne([FromUri] int operationId, [FromUri] string tsc,
[FromUri] string frc, [FromUri] string frm, [FromUri] int userId, [FromBody] NamesDto n, string ricevutaRitorno)
        {

            //UTENTE INSERITORE
            var u = _context.Users.SingleOrDefault(a => a.id == userId);

            //INSERISCI SENDER
            //var sender = _context.SendersUsers.FirstOrDefault(a => a.id == senderId);
            //SenderDtos ss = Mapper.Map<SendersUsers, SenderDtos>(sender);
            //ss.operationId = operationId;
            //int sId = SenderController.CreateItem(ss);

            //INSERISCI NAMES

            NamesDtos nos = Mapper.Map<NamesDto, NamesDtos>(n);
            nos.operationId = operationId;
            nos.requestId = null;
            nos.guidUser = null;
            nos.valid = true;

            bool fronteRetro = true;
            if (frc == "0")
                fronteRetro = false;

            bool tipoStampa = true;
            if (tsc == "0")
                tipoStampa = false;

            bool rr = true;
            if (ricevutaRitorno == "0")
                rr = false;

            nos.fronteRetro = fronteRetro;
            nos.ricevutaRitorno = rr;
            nos.tipoStampa = tipoStampa;

            nos.insertDate = DateTime.Now;
            nos.currentState = (int)currentState.inAttesa;

            //NUMERO DI PAGINE
            //ComponentInfo.SetLicense("ADWG-YKI0-D7LE-5JK9");
            //var document = PdfDocument.Load(nos.fileName);
            //nos.numberOfPages = document.Pages.Count();

            var nc = new NamesController();
            int idName = nc.CreateItem(nos, u.userPriority);


            return idName;
        }

    }
}
