using Api.Model;
using Api.Models;
using Api.ServiceCOL;
using Api.ServiceMOL;
using Api.ServiceRol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Api.Controllers.Api
{
    [RoutePrefix("api/Automations")]
    public class AutomationsController : ApiController
    {
        private static Entities _context;

        public AutomationsController()
        {
            _context = new Entities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        [Route("CreateAragFileMol")]
        [HttpGet]
        public async Task<string> CreateAragFileMol()
        {
            var typeSped = "Mol";
            var r = new Responses();
            var l = new List<CutNames>();
            try
            {
                var domani = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                var xGiorniFa = Convert.ToDateTime(DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd"));

                var names = _context.Names
                    .Where(a => a.Operations.userId == 410 || a.Operations.Users.parentId == 410)
                    .Where(a => a.currentState == (int)currentState.PresoInCarico || a.currentState == (int)currentState.InLavorazione)
                    .Where(a => a.stato == "Accettato OnLine" || a.stato == "Presa in carico Poste")
                    .Where(a => a.valid == true)
                    .Where(a => a.codice != "")
                    .Where(a => a.insertDate >= xGiorniFa)
                    .Where(a => a.insertDate < domani)
                    .Where(a => a.codice != null)
                    .Where(a => a.finalState != true)
                    .Where(a=>a.operationType == (int)operationType.MOL)
                    .ToList();

                var guidUser = names.Select(a => a.Operations.Users.guidUser).FirstOrDefault();
                var contractCode = names.Select(a => a.Operations.Users.CodiceContrattoMOL).FirstOrDefault();

                foreach (var n in names)
                {
                    var arrNames = n.fileName.Split('/');
                    r = StatoMol(guidUser, n.requestId, contractCode);
                    if (n.stato != r.stato)
                    {
                        n.stato = r.stato;
                        //n.consegnatoDate = DateTime.Parse(r.dataEsito);
                        _context.SaveChanges();
                    }
                    var c = new CutNames()
                    {
                        RagioneSociale = n.businessName,
                        Nome = n.name,
                        Cognome = n.surname,
                        Cap = n.cap,
                        Citta = n.city,
                        Provincia = n.province,
                        Nazione = n.state,
                        Indirizzo = n.address,
                        CompletamentoIndirizzo = n.complementAddress,
                        CompletamentoNominativo = n.complementNames,
                        NomeFile = arrNames[arrNames.Length - 1],
                        CodiceFiscale = n.fiscalCode,
                        Telefono = n.mobile,
                        DataAccettazione = n.presaInCaricoDate.ToString(),
                        Stato = r.stato,
                        NumeroAccettazione = n.codice,
                        DataEsitoFinale = r.dataEsito,
                    };
                    l.Add(c);
                }
                 creaFile(l,typeSped);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
            return "OK";
        }

        [Route("CreateAragFileCol")]
        [HttpGet]
        public async Task<string> CreateAragFileCol1(int start, int end)
        {
            var typeSped = "";
            switch (start)
            {
                case 60:
                    typeSped = "Col1";
                    break;
                case 39:
                    typeSped = "Col2";
                    break;
                case 20:
                    typeSped = "Col3";
                    break;
            }

            var r = new Responses();
            var l = new List<CutNames>();
            try
            {
                var domani = Convert.ToDateTime(DateTime.Now.AddDays(-end).ToString("yyyy-MM-dd"));
                var xGiorniFa = Convert.ToDateTime(DateTime.Now.AddDays(-start).ToString("yyyy-MM-dd"));

                var names = _context.Names
                    .Where(a => a.Operations.userId == 410 || a.Operations.Users.parentId == 410)
                    .Where(a => a.currentState == (int)currentState.PresoInCarico || a.currentState == (int)currentState.InLavorazione)
                    .Where(a => a.stato == "Accettato OnLine" || a.stato == "Presa in carico Poste")
                    .Where(a => a.valid == true)
                    .Where(a => a.codice != "")
                    .Where(a => a.insertDate >= xGiorniFa)
                    .Where(a => a.insertDate < domani)
                    .Where(a => a.codice != null)
                    .Where(a => a.finalState != true)
                    .Where(a => a.operationType == (int)operationType.COL)
                    .ToList();

                var guidUser = names.Select(a => a.Operations.Users.guidUser).FirstOrDefault();
                var contractCode = names.Select(a => a.Operations.Users.CodiceContrattoCOL).FirstOrDefault();

                foreach (var n in names)
                {
                    var arrNames = n.fileName.Split('/');
                    r = StatoCol(guidUser, n.requestId, contractCode);
                    if (n.stato != r.stato)
                    {
                        n.stato = r.stato;
                        //n.consegnatoDate = DateTime.Parse(r.dataEsito);
                        _context.SaveChanges();
                    }

                    var c = new CutNames()
                    {
                        RagioneSociale = n.businessName,
                        Nome = n.name,
                        Cognome = n.surname,
                        Cap = n.cap,
                        Citta = n.city,
                        Provincia = n.province,
                        Nazione = n.state,
                        Indirizzo = n.address,
                        CompletamentoIndirizzo = n.complementAddress,
                        CompletamentoNominativo = n.complementNames,
                        NomeFile = arrNames[arrNames.Length - 1],
                        CodiceFiscale = n.fiscalCode,
                        Telefono = n.mobile,
                        DataAccettazione = n.presaInCaricoDate.ToString(),
                        Stato = r.stato,
                        NumeroAccettazione = n.codice,
                        DataEsitoFinale = r.dataEsito,
                    };
                    l.Add(c);
                }
                creaFile(l, typeSped);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
            return "OK";
        }

        [Route("MergeFile")]
        [HttpGet]
        public async Task<string> MergeAragFile()
        {
            var folder = HttpContext.Current.Server.MapPath("/Public/Export");
            var outputFile = "Export-" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv";
            var filePath = HttpContext.Current.Server.MapPath("/Public/Export/" + outputFile);

            MergeFile(folder, filePath);

            var fl = folder + "/" + outputFile;


            GlobalClass.SendViaFtp(folder + "/" + outputFile, outputFile, "FtpArag2021", "qi72z8Q_", "ftp://ftp.easyway.technology/Export/");
            //GlobalClass.SendViaFtp(folder + "/" + outputFile, outputFile, "AdminScialby", "Sugna29!", "ftp://AdminScialby@185.81.4.76/ScambioFile/");


            var destfileName = HttpContext.Current.Server.MapPath("/Public/Export/Worked/");
            var fileNameDest = fl.Split('/').Last();
            File.Move(fl, destfileName+fileNameDest);
            File.Delete(folder + "/" + outputFile);

            return "ok";
        }




        private Responses StatoRol(Guid guidUser, string requestId)
        {
            var r = new Responses();

            ServiceRol.Richiesta IdRol = new ServiceRol.Richiesta();
            IdRol.GuidUtente = "";
            IdRol.IDRichiesta = requestId;

            ServiceRol.Richiesta[] Richiesta = new ServiceRol.Richiesta[1];
            Richiesta[0] = IdRol;

            ROLServiceSoapClient service = new ROLServiceSoapClient();
            service = getNewServiceRol(guidUser);

            StatoInviiPerIDResult stato = service.StatoInviiPerID(Richiesta);
            if (stato.ArrayDiRichieste == null)
                return r;

            if (stato.ArrayDiRichieste[0].StatoRichieste == null)
                return r;

            StatoRichiesta sr = stato.ArrayDiRichieste[0].StatoRichieste[0];

            r.dataEsito = sr.DataEsito;
            r.stato = sr.StatoDescrizione;

            return r;

        }

        private Responses StatoCol(Guid guidUser, string requestId, string codiceContrattoMol)
        {
            var r = new Responses();

            PostaContestServiceClient service = new PostaContestServiceClient();
            service = GetNewServiceCol(guidUser);
            var s = service.RecuperaEsitiPerIdRichiesta(new ServiceCOL.RecuperaEsitiPerIdRichiestaRequest()
            {
                IdRichieste = new[] { requestId },
                CodiceContratto = codiceContrattoMol
            });
            if (s.Esito == ServiceCOL.EsitoPostaEvo.OK)
            {
                r.stato = s.RendicontazioneEsiti[0].DescrizioneEsito;
                r.dataEsito = s.RendicontazioneEsiti[0].DataEsito.ToString("dd/MM/yyyy");
            }
            return r;
        }

        private Responses StatoMol(Guid guidUser, string requestId, string codiceContrattoMol)
        {
            var r = new Responses();

            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            service = GetNewServiceMol(guidUser);

            var s = service.RecuperaEsitiPerIdRichiesta(new ServiceMOL.RecuperaEsitiPerIdRichiestaRequest()
            {
                IdRichieste = new[] { requestId },
                CodiceContratto = codiceContrattoMol
            });

            if (s.Esito == ServiceMOL.EsitoPostaEvo.OK)
            {
                r.stato = s.RendicontazioneEsiti[0].DescrizioneEsito;
                r.dataEsito = s.RendicontazioneEsiti[0].DataEsito.ToString("dd/MM/yyyy");
            }

            return r;
        }

        private static ROLServiceSoapClient getNewServiceRol(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;
            ROLServiceSoapClient service = new ROLServiceSoapClient();
            service.ClientCredentials.UserName.UserName = Users.usernamePoste;
            service.ClientCredentials.UserName.Password = Users.pwdPoste;
            return service;
        }
        private static PostaContestServiceClient GetNewServiceCol(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;

            PostaContestServiceClient service = new PostaContestServiceClient();
            service.ClientCredentials.UserName.UserName = Users.usernamePoste;
            service.ClientCredentials.UserName.Password = Users.pwdPoste;

            return service;
        }
        private static RaccomandataMarketServiceClient GetNewServiceMol(Guid guidUser)
        {
            //MULTIPLE USERS
            var u = _context.Users.Where(a => a.guidUser == guidUser);
            if (u.Count() == 0)
                return null;

            var Users = _context.Users.Where(a => a.guidUser == guidUser).SingleOrDefault(a => a.parentId == 0);
            if (Users == null)
                return null;

            RaccomandataMarketServiceClient service = new RaccomandataMarketServiceClient();
            service.ClientCredentials.UserName.UserName = Users.usernamePoste;
            service.ClientCredentials.UserName.Password = Users.pwdPoste;

            return service;
        }
        public static string CustomerToString(Names c)
       {
            var d = c.consegnatoDate.ToString();
            if (d == null)
                d = DateTime.Now.ToString();

            var x =
                c.businessName.ToString() + ";" +
                c.name + ";" +
                c.surname + ";" +
                c.cap.ToString() + ";" +
                c.city.ToString() + ";" +
                c.province.ToString() + ";" +
                c.state.ToString() + ";" +
                c.address.ToString() + ";" +
                c.complementAddress.ToString() + ";" +
                c.complementNames.ToString() + ";" +
                c.fileName.ToString() + ";" +
                c.fiscalCode.ToString() + ";" +
                c.mobile.ToString() + ";" +
                c.insertDate.ToString() + ";" +
                c.stato.ToString() + ";" +
                c.codice.ToString() + ";" +
                 d + "\n";

            return x;
        }

        private void creaFile(List<CutNames> namesN, string typeSped)
        {
            var csv = "RagioneSociale;Nome;Cognome;CAP;Citta;Provincia;Stato;Indirizzo;CompletamentoIndirizzo;CompletamentoNominativo;NomeFile;CodiceFiscale;Telefono;DataAccettazione;Stato;NumeroAccettazione;DataEsitoFinale\n";
            var newFile = "Export-" + DateTime.Now.ToString("dd-MM-yyyy") + typeSped + ".csv";
            foreach (var c in namesN)
            {
                //var arrNames = c.fileName.Split('/');
                csv += c.RagioneSociale + ";" + 
                    c.Nome + ";" + c.Cognome + ";" + c.Cap + ";" + c.Citta + ";" + 
                    c.Provincia + ";" + c.Nazione + ";" + c.Indirizzo + ";" + c.CompletamentoIndirizzo + ";" + 
                    c.CompletamentoNominativo + ";" + c.NomeFile + ";" + c.CodiceFiscale + ";" + c.Telefono + ";" + 
                    Convert.ToDateTime(c.DataAccettazione).ToString("dd/MM/yyyy") + ";" + c.Stato + ";" + c.NumeroAccettazione + ";" + c.DataEsitoFinale + "\n";
            }

            var filePath = HttpContext.Current.Server.MapPath("/Public/Export/" + newFile);
            File.WriteAllText(filePath, csv.ToString());

        }

        private void MergeFile(string folder, string outputFile)
        {
            //var csv = "RagioneSociale;Nome;Cognome;CAP;Citta;Provincia;Stato;Indirizzo;CompletamentoIndirizzo;CompletamentoNominativo;NomeFile;CodiceFiscale;Telefono;DataAccettazione;Stato;NumeroAccettazione;DataEsitoFinale\n";
            string[] csvFiles = Directory.GetFiles(folder, "*.csv");
            //var filePath = HttpContext.Current.Server.MapPath("/Public/Export/" + outputFile);
            using (StreamWriter writer = new StreamWriter(outputFile))
            {
                bool headerWritten = false;
                foreach (string fl in csvFiles)
                {
                    string[] lines = File.ReadAllLines(fl);

                    // Se la riga di intestazione non è stata ancora scritta, scrivila nel file di output
                    if (!headerWritten)
                    {
                        writer.WriteLine(lines[0]);
                        headerWritten = true;
                    }

                    // Scrivi le righe rimanenti nel file di output, saltando la prima riga
                    for (int i = 1; i < lines.Length; i++)
                    {
                        writer.WriteLine(lines[i]);
                    }

                    var surcefileName = fl;
                    var destfileName = HttpContext.Current.Server.MapPath("/Public/Export/Worked/");
                    var fileName = fl.Split('\\').Last();

                    File.Move(surcefileName, destfileName + fileName);
                    File.Delete(fl);
                }

            }


        }
    }
}
