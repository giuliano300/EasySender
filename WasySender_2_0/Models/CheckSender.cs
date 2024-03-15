using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class CheckSender
    {
       public static ControlloMittente verificaMittente(Sender s)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();

            ControlloMittente ctrl = new ControlloMittente();
            ctrl.sender = s;

            string Cap = s.cap.Replace(" ", "");
            var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
            int i = comune.Count();

            // CONTROLLO CAP
            crt crt = Check.verificaCap(Cap, i);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Mittente";
            }

            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = Check.verificaRagioneSociale(s.businessName, s.name, s.surname);
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Mittente";
            }

            // CONTROLLO INDIRIZZO
            crt crtA = Check.verificaIndirizzo(s.dug, s.address, s.houseNumber);
            if (!crtA.Valido)
            {
                ctrl.Valido = crtA.Valido;
                ctrl.Errore = crtA.Errore + " - Mittente";
            }
            // CONTROLLO CITTA'
            crt crtC = Check.verificaCitta(s.city);
            if (!crtC.Valido)
            {
                ctrl.Valido = crtC.Valido;
                ctrl.Errore = crtC.Errore + " - Mittente";
            }

            // CONTROLLO PROVINCIA
            crt crtP = Check.verificaProvincia(s.province);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Mittente";
            }

            // CONTROLLO STATO
            crt crtS = Check.verificaStato(s.state);
            if (!crtS.Valido)
            {
                ctrl.Valido = crtS.Valido;
                ctrl.Errore = crtS.Errore + " - Mittente";
            }

            return ctrl;
        }
    }
}