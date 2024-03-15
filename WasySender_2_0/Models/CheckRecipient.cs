using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WasySender_2_0.DataModel;

namespace WasySender_2_0.Models
{
    public class CheckRecipient
    {
       public static ControlloDestinatario verificaDestinatario(NamesLists recipient, IEnumerable<ComuniXLS> comune, bool verificaFileName = false)
        {
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();

            NamesLists Destinatario = recipient;
            ControlloDestinatario ctrl = new ControlloDestinatario();
            ctrl.Destinatario = Destinatario;

            string Cap = Destinatario.cap.Replace(" ", "");
            int i = comune.Count();

            // CONTROLLO CAP
            crt crt = Check.verificaCap(Cap, i, Destinatario.state);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Destinatario";
            }

            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = Check.verificaRagioneSociale(Destinatario.businessName, Destinatario.name, Destinatario.surname);
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Destinatario";
            }

            // CONTROLLO INDIRIZZO
            crt crtA = Check.verificaIndirizzo(Destinatario.dug, Destinatario.address, Destinatario.houseNumber, Destinatario.state);
            if (!crtA.Valido)
            {
                ctrl.Valido = crtA.Valido;
                ctrl.Errore = crtA.Errore + " - Destinatario";
            }
            // CONTROLLO CITTA'
            crt crtC = Check.verificaCitta(Destinatario.city);
            if (!crtC.Valido)
            {
                ctrl.Valido = crtC.Valido;
                ctrl.Errore = crtC.Errore + " - Destinatario";
            }
            // CONTROLLO CF
            crt crtCf = Check.controlloFormaleCf(Destinatario.fiscalCode);
            if (!crtCf.Valido)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Codice fiscale - Destinatario";
            }

            // CONTROLLO PROVINCIA
            crt crtP = Check.verificaProvincia(Destinatario.province, Destinatario.state);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Destinatario";
            }

            // CONTROLLO CAP - COMUNE - PROVINCIA
            if (Destinatario.state.ToLower() == "italia") { 
                if (comune.Count() > 0) { 
                    crt crtCCP = Check.verificaCapComuneProvincia(comune.ToList(), Destinatario.cap, Destinatario.city, Destinatario.province);
                    if (!crtCCP.Valido)
                    {
                        ctrl.Valido = crtCCP.Valido;
                        ctrl.Errore = crtCCP.Errore + " - Destinatario";
                    }
                }
                else
                {
                    ctrl.Valido = false;
                    ctrl.Errore = "Cap non corretto - Destinatario";
                }
            }

            // CONTROLLO STATO
            crt crtS = Check.verificaStato(Destinatario.state);
            if (!crtS.Valido)
            {
                ctrl.Valido = crtS.Valido;
                ctrl.Errore = crtS.Errore + " - Destinatario";
            }

            //CONTROLLO FILENAME
            if (verificaFileName) {
                crt crtF = Check.verificaFileName(Destinatario.fileName);
                if (!crtF.Valido)
                {
                    ctrl.Valido = crtF.Valido;
                    ctrl.Errore = crtF.Errore + " - Destinatario";
                }
            }
            return ctrl;
        }
        public static ControlloDestinatario verificaDestinatarioPacchi(NamesLists recipient, IEnumerable<ComuniXLS> comune, bool verificaFileName = false)
        {
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();

            NamesLists Destinatario = recipient;
            ControlloDestinatario ctrl = new ControlloDestinatario();
            ctrl.Destinatario = Destinatario;

            string Cap = Destinatario.cap.Replace(" ", "");
            int i = comune.Count();

            // CONTROLLO CAP
            crt crt = Check.verificaCap(Cap, i, Destinatario.state);
            if (!crt.Valido)
            {
                ctrl.Valido = crt.Valido;
                ctrl.Errore = crt.Errore + " - Destinatario";
            }

            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = Check.verificaRagioneSociale(Destinatario.businessName, Destinatario.name, Destinatario.surname);
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Destinatario";
            }

            // CONTROLLO INDIRIZZO
            crt crtA = Check.verificaIndirizzo(Destinatario.dug, Destinatario.address, Destinatario.houseNumber, Destinatario.state);
            if (!crtA.Valido)
            {
                ctrl.Valido = crtA.Valido;
                ctrl.Errore = crtA.Errore + " - Destinatario";
            }
            // CONTROLLO CITTA'
            crt crtC = Check.verificaCitta(Destinatario.city);
            if (!crtC.Valido)
            {
                ctrl.Valido = crtC.Valido;
                ctrl.Errore = crtC.Errore + " - Destinatario";
            }

            // CONTROLLO PROVINCIA
            crt crtP = Check.verificaProvincia(Destinatario.province, Destinatario.state);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Destinatario";
            }

            // CONTROLLO CAP - COMUNE - PROVINCIA
            if (Destinatario.state.ToLower() == "italia")
            {
                if (comune.Count() > 0)
                {
                    crt crtCCP = Check.verificaCapComuneProvincia(comune.ToList(), Destinatario.cap, Destinatario.city, Destinatario.province);
                    if (!crtCCP.Valido)
                    {
                        ctrl.Valido = crtCCP.Valido;
                        ctrl.Errore = crtCCP.Errore + " - Destinatario";
                    }
                }
                else
                {
                    ctrl.Valido = false;
                    ctrl.Errore = "Cap non corretto - Destinatario";
                }
            }

            // CONTROLLO STATO
            crt crtS = Check.verificaStato(Destinatario.state);
            if (!crtS.Valido)
            {
                ctrl.Valido = crtS.Valido;
                ctrl.Errore = crtS.Errore + " - Destinatario";
            }

            // CONTROLLO DATA
            crt crtDa = Check.verificaShipmentData(Destinatario.shipmentDate.ToString());
            if (!crtDa.Valido)
            {
                ctrl.Valido = crtDa.Valido;
                ctrl.Errore = crtDa.Errore + " - Destinatario";
            }

            // CONTROLLO MISURE
            crt crtM = Check.verificaMisure((int)Destinatario.weight, (int)Destinatario.height, (int)Destinatario.length, (int)Destinatario.width);
            if (!crtM.Valido)
            {
                ctrl.Valido = crtM.Valido;
                ctrl.Errore = crtM.Errore + " - Destinatario";
            }



            return ctrl;
        }
        public static async Task<ControlloDestinatario> verificaDestinatarioVisure(NamesLists recipient)
        {
            List<ControlloDestinatario> listctrl = new List<ControlloDestinatario>();

            NamesLists Intestatario = recipient;
            ControlloDestinatario ctrl = new ControlloDestinatario();
            ctrl.Destinatario = Intestatario;


            // CONTROLLO RAGIONE SOCIALE / NOME E COGNOME
            crt crtR = Check.verificaRagioneSociale(Intestatario.businessName, "", "");
            if (!crtR.Valido)
            {
                ctrl.Valido = crtR.Valido;
                ctrl.Errore = crtR.Errore + " - Intestatario";
            }

            //SE INIZIA CON IT O CON UN NUMERO CONTROLLO PARTITA IVA
            if (Intestatario.fiscalCode.StartsWith("IT"))
            {
                //var code = Intestatario.fiscalCode.Replace("IT", "");
                //HttpResponseMessage get = new HttpResponseMessage();
                //get = await Globals.HttpClientSend("GET", "Vies?vatNumber=" + code, false);
                //if (!get.IsSuccessStatusCode)
                //{
                //    ctrl.Valido =false;
                //    ctrl.Errore =  "Errore nella convalida della partita iva - Intestatario";
                //}
                //else
                //{
                //    var res = await get.Content.ReadAsAsync<checkVatResponse>();
                //    if(res == null)
                //    {
                //        ctrl.Valido = false;
                //        ctrl.Errore = "Partita iva non valida - Intestatario";
                //    }
                //    else 
                //    { 
                //        if (!res.Body.valid)
                //        {
                //            ctrl.Valido = res.Body.valid;
                //            ctrl.Errore = "Partita iva non valida - Intestatario";
                //        }
                //    }
                //}
            }
            else 
            { 
                crt crtCf = Check.controlloFormaleCf(Intestatario.fiscalCode);
                if (Intestatario.fiscalCode.Length <= 0)
                {
                    ctrl.Valido = crtCf.Valido;
                    ctrl.Errore = crtCf.Errore +  " - Intestatario";
                }
                if (!crtCf.Valido)
                {
                    ctrl.Valido = crtCf.Valido;
                    ctrl.Errore = crtCf.Errore + " - Intestatario";
                }
            }

            // CONTROLLO PROVINCIA
            crt crtP = Check.verificaCCIA(Intestatario.province);
            if (!crtP.Valido)
            {
                ctrl.Valido = crtP.Valido;
                ctrl.Errore = crtP.Errore + " - Intestatario";
            }


            return ctrl;
        }
    }
}