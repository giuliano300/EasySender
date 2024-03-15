using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class CheckBulletin
    {
        public static ControlloBollettino verificaBollettino(Bulletins b)
        {
            var r = File.ReadAllText(HttpContext.Current.Server.MapPath("~/json/comuniItaliani.json"));
            var comuni = JsonConvert.DeserializeObject<List<ComuniItaliani>>(r);

            string Cap = b.EseguitoDaCAP.Replace(" ", "");
            var comune = comuni.Where(c => c.cap.ToString().Contains(Cap));
            int i = comune.Count();
            
            ControlloBollettino ctrl = new ControlloBollettino();
            ctrl.Bollettino = b;

            // CONTROLLO CONTO CORRENTE
            if (b.NumeroContoCorrente == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente non valido - Bollettino";
                return ctrl;
            }

            if (!Globals.onlyNumbers(b.NumeroContoCorrente))
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente deve contenere solo numeri";
                return ctrl;
            }

            if (b.NumeroContoCorrente.Length != 12)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo NumerContoCorrente non ha la lunghezza consentita di 12 numeri";
                return ctrl;
            }

            // Intestato A
            if (b.IntestatoA == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo IntestatoA vuoto - Bollettino";
                return ctrl;
            }

            // CodiceCliente
            if (b.PagoPA != true) { 
                crt crtCC = Check.verificaCodiceCliente(b.CodiceCliente);
                if (!crtCC.Valido)
                {
                    ctrl.Valido = false;
                    ctrl.Errore = crtCC.Errore;
                    return ctrl;
                }
            }

            // ImportoEuro
            if (b.ImportoEuro == 0)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo ImportoEuro vuoto - Bollettino";
                return ctrl;
            }

            // EseguitoDaNominativo
            if (b.EseguitoDaNominativo == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaNominativo vuoto - Bollettino";
                return ctrl;
            }

            // EseguitoDaIndirizzo
            if (b.EseguitoDaIndirizzo == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaIndirizzo vuoto - Bollettino";
                return ctrl;
            }

            // EseguitoDaIndirizzo
            crt crt = Check.verificaCap(b.EseguitoDaCAP, i);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Bollettino";
                return ctrl;
            }

            // EseguitoDaLocalita
            if (b.EseguitoDaLocalita == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo EseguitoDaLocalita vuoto - Bollettino";
                return ctrl;
            }

            // Causale
            if (b.Causale == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo Causale vuoto - Bollettino";
                return ctrl;
            }

            // BulletinType
            if (b.BulletinType != (int)bulletinType.Bollettino451 && b.BulletinType != (int)bulletinType.Bollettino674 && b.BulletinType != (int)bulletinType.Bollettino896 && b.BulletinType != (int)bulletinType.PagoPA)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo BulletinType errato - Bollettino";
                return ctrl;
            }


            return ctrl;
        }

    }
}