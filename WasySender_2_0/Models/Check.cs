using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace WasySender_2_0.Models
{
    public class Check
    {
        public static crt verificaCap(string Cap, int i, string state = "italia")
        {
            crt ctrl = new crt();
            if (state.ToLower() != "italia")
                return ctrl;

            if (Cap.Length == 0)
            {
                ctrl.Errore = "Cap inesistente";
                ctrl.Valido = false;
            }
            if (Cap.Length != 5)
            {
                ctrl.Errore = "Lughezza cap non valida";
                ctrl.Valido = false;
            }

            if (i == 0)
            {
                ctrl.Errore = "Cap non valido";
                ctrl.Valido = false;
            }
            return ctrl;
        }

        public static crt verificaCodiceCliente(string codiceCliente)
        {
            crt ctrl = new crt();
            // CodiceCliente
            if (codiceCliente == "")
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo CodiceCliente vuoto - Bollettino";
                return ctrl;
            }

            // CodiceCliente
            if (!Globals.onlyNumbers(codiceCliente))
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo CodiceCliente deve contenere solo numeri";
                return ctrl;
            }

            // CodiceCliente
            if (codiceCliente.Length != 18)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Campo CodiceCliente non ha la lunghezza consentita di 18 numeri";
                return ctrl;
            }

            long firstCod = Convert.ToInt64(codiceCliente.Substring(0, 16));
            int lastCod = Convert.ToInt32(codiceCliente.Substring(codiceCliente.Length - 2, 2));

            long codiceControllo = firstCod % 93;
            if (codiceControllo != lastCod)
            {
                ctrl.Valido = false;
                ctrl.Errore = "Nel Campo CodiceCliente il controcodice non è valido (Primi 16 caratteri mod 93)";
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaRagioneSociale(string ragioneSociale = "", string nome = "", string cognome = "")
        {
            crt ctrl = new crt();
            if (ragioneSociale.Length > 44)
            {
                ctrl.Errore = "Ragione sociale più di 44 caratteri";
                ctrl.Valido = false;
            }
            if (ragioneSociale.Length == 0 & (nome.Length == 0 | cognome.Length == 0))
            {
                ctrl.Errore = "Inserire Nome e Cognome o Ragione Sociale";
                ctrl.Valido = false;
            }
            return ctrl;
        }

        public static crt verificaIndirizzo(string dug, string indirizzo, string numeroCivico = "", string state = "")
        {
            crt ctrl = new crt();
            if (indirizzo.Length == 0)
            {
                ctrl.Errore = "Indirizzo vuoto";
                ctrl.Valido = false;
            }
            if (indirizzo.Length > 44)
            {
                ctrl.Errore = "Indirizzo supera la lunghezza massima. Utilizzare il campo complemento indirizzo.";
                ctrl.Valido = false;
            }
            //else
            //{
            //    // CONTROLLO NUMERO CIVICO
            //    if (numeroCivico.Length > 5)
            //    {
            //        ctrl.Errore = "il numero civico supera i 5 caratteri";
            //        ctrl.Valido = false;
            //    }

            //    if (dug.Length <= 0 && state.ToLower() == "italia")
            //    {
            //        ctrl.Errore = "il DUG non è valido";
            //        ctrl.Valido = false;
            //    }
            //    if (dug.Length > 10)
            //    {
            //        ctrl.Errore = "il DUG supera i 10 caratteri";
            //        ctrl.Valido = false;
            //    }
            //}
            return ctrl;
        }

        public static crt verificaCitta(string city)
        {
            crt ctrl = new crt();
            if (city.Length == 0)
            {
                ctrl.Errore = "città vuota";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaShipmentData(string data)
        {
            crt ctrl = new crt();
            if (data.Length == 0)
            {
                ctrl.Errore = "data invio vuota";
                ctrl.Valido = false;
                return ctrl;
            }

            var d = Convert.ToDateTime(data);
            if (d < DateTime.Now)
            {
                ctrl.Errore = "la data invio non può essere antecedente ad oggi";
                ctrl.Valido = false;
            }

            return ctrl;
        }

        public static crt verificaMisure(int weight, int height, int length, int width)
        {
            crt ctrl = new crt();
            if (weight <= 0)
            {
                ctrl.Errore = "Peso vuoto";
                ctrl.Valido = false;
                return ctrl;
            }

            if (height <= 0)
            {
                ctrl.Errore = "Altezza vuota";
                ctrl.Valido = false;
                return ctrl;
            }

            if (length <= 0)
            {
                ctrl.Errore = "Lunghezza vuota";
                ctrl.Valido = false;
                return ctrl;
            }

            if (width <= 0)
            {
                ctrl.Errore = "Profondità vuota";
                ctrl.Valido = false;
                return ctrl;
            }

            if (!Globals.onlyNumbers(weight.ToString()))
            {
                ctrl.Errore = "Peso deve essere un intero";
                ctrl.Valido = false;
                return ctrl;
            }

            if (!Globals.onlyNumbers(height.ToString()))
            {
                ctrl.Errore = "Altezza deve essere un intero";
                ctrl.Valido = false;
                return ctrl;
            }

            if (!Globals.onlyNumbers(length.ToString()))
            {
                ctrl.Errore = "Lunghezza deve essere un intero";
                ctrl.Valido = false;
                return ctrl;
            }

            if (!Globals.onlyNumbers(width.ToString()))
            {
                ctrl.Errore = "Profondità deve essere un intero";
                ctrl.Valido = false;
                return ctrl;
            }


            return ctrl;
        }

        public static crt verificaProvincia(string provincia, string state = "italia")
        {
            crt ctrl = new crt();
            if (state.ToLower() != "italia")
                return ctrl;

            if (provincia.Length == 0)
            {
                ctrl.Errore = "provincia vuota";
                ctrl.Valido = false;
                return ctrl;
            }
            if (provincia.Length > 2)
            {
                ctrl.Errore = "il campo provincia deve avere 2 caratteri";
                ctrl.Valido = false;
                return ctrl;
            }
            if (!Globals.onlyLetters(provincia))
            {
                ctrl.Errore = "nel campo provincia sono inseriti caratteri non validi";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaCCIA(string provincia)
        {
            crt ctrl = new crt();

            if (provincia.Length == 0)
            {
                ctrl.Errore = "CCIA vuota";
                ctrl.Valido = false;
                return ctrl;
            }
            if (provincia.Length > 2)
            {
                ctrl.Errore = "il campo CCIA deve avere 2 caratteri";
                ctrl.Valido = false;
                return ctrl;
            }
            if (!Globals.onlyLetters(provincia))
            {
                ctrl.Errore = "nel campo CCIA sono inseriti caratteri non validi";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }
        public static crt verificaStato(string stato)
        {
            crt ctrl = new crt();
            if (stato.Length == 0)
            {
                ctrl.Errore = "stato vuoto";
                ctrl.Valido = false;
                return ctrl;
            }
            //if (!Globals.onlyLetters(stato))
            //{
            //    ctrl.Errore = "nel campo stato sono inseriti caratteri non validi";
            //    ctrl.Valido = false;
            //    return ctrl;
            //}
            if(stato.ToUpper() != "ITALIA") {
                if (stato.ToUpper().Contains("ITA"))
                {
                    ctrl.Errore = "Il campo stato ITALIA deve essere scritto per esteso";
                    ctrl.Valido = false;
                    return ctrl;
                }
                var n = Globals.GetNazioniList();
                var us = stato.ToUpper();
                if (!n.Contains(us))
                {
                    ctrl.Errore = "Il campo stato non corrisponde a nessuno stato disponibile";
                    ctrl.Valido = false;
                    return ctrl;
                }
            }
            return ctrl;
        }

        public static crt verificaFileName(string fileName)
        {
            crt ctrl = new crt();
            if (fileName == "" || fileName == null)
            {
                ctrl.Errore = "file vuoto";
                ctrl.Valido = false;
                return ctrl;
            }

            var ex = fileName.Split('.');

            if (ex.Length < 2)
            {
                ctrl.Errore = "l'estensione del file non è consentita";
                ctrl.Valido = false;
                return ctrl;
            }

            if (ex[ex.Length-1].ToUpper() != "PDF")
            {
                ctrl.Errore = "l'estensione del file non è consentita";
                ctrl.Valido = false;
                return ctrl;
            }

            return ctrl;
        }

        public static crt verificaDimensioneFile(string path, bool fronteRetro = false)
        {
            crt ctrl = new crt();
            try 
            { 
                PdfReader pdfReader = new PdfReader(path);
                int numberOfPages = pdfReader.NumberOfPages;
                long weigth = pdfReader.FileLength / 1024;
                pdfReader.Close();
                if (weigth >= Globals.limiteMaxPeso)
                {
                    ctrl.Errore = "la dimensione del file eccede quella consentita(5MB)";
                    ctrl.Valido = false;
                    return ctrl;
                }
                int limiteMax = Globals.limiteMaxPagine;
                if (fronteRetro)
                    limiteMax = limiteMax * 2;
                if (numberOfPages > limiteMax)
                {
                    ctrl.Errore = "il numero di pagine del file eccede quello consentito(18 pagine)";
                    ctrl.Valido = false;
                    return ctrl;
                }
                    //var format = Globals.GetPdfType(path);
                    //if (format != "A4") {
                    //    ctrl.Errore = "file non in formato a4";
                    //    ctrl.Valido = false;
                    //    return ctrl;
                    //}
            }
            catch(Exception e)
            {
                var j = e;
            }
            return ctrl;
        }
        public static crt verificaDimensioneFileLogo(string path)
        {
            crt ctrl = new crt();
            return ctrl;
        }

        public static crt verificaCapComuneProvincia(List<ComuniXLS> comune, string cap, string city, string province)
        {
            crt ctrl = new crt();
            if (comune.Where(a => a.cap.Contains(cap)).Count() == 0)
            {
                ctrl.Errore = "i dati cap/provincia/comune non sono congruenti!";
                ctrl.Valido = false;
                return ctrl;
            }
            var cl = city.ToLower();
            if (comune.Where(a => a.comune.ToLower().Contains(cl)).Count() == 0)
            {
                ctrl.Errore = "i dati cap/provincia/comune non sono congruenti!";
                ctrl.Valido = false;
                return ctrl;
            }
            var sig = province.ToLower();
            if (comune.Where(a => a.sigla.ToLower().Contains(sig)).Count() == 0)
            {
                ctrl.Errore = "i dati cap/provincia/comune non sono congruenti!";
                ctrl.Valido = false;
                return ctrl;
            }
            return ctrl;
        }

        public static crt controlloFormaleCf(string cf)
        {
            var crt = new crt();
            if (String.IsNullOrEmpty(cf))
            {
                crt.Valido = true;
                return crt;
            }

            Regex CheckRegex = new Regex(@"^[A-Z]{6}[\d]{2}[A-Z][\d]{2}[A-Z][\d]{3}[A-Z]$");

            if (cf.Length < 16) {
                crt.Errore = "Il codice fiscale non rispetta la lunghezza minima.";
                crt.Valido = false;
                return crt;
            };

            cf = Normalize(cf, false);
            string cf_NoOmocodia = string.Empty;
            if (!CheckRegex.Match(cf).Success)
            {
                cf_NoOmocodia = SostituisciLettereOmocodia(cf);
                if (!CheckRegex.Match(cf_NoOmocodia).Success)
                {
                    crt.Errore = "Il codice fiscale non è nella forma corretta.";
                    crt.Valido = false;
                    return crt;
                };
                // invalid Fiscal Code
            }

            return crt;
        }

        private static string Normalize(string s, bool normalizeDiacritics)
        {
            if (String.IsNullOrEmpty(s)) return s;
            s = s.Trim().ToUpper();
            if (normalizeDiacritics)
            {
                string src = "ÀÈÉÌÒÙàèéìòù";
                string rep = "AEEIOUAEEIOU";
                for (int i = 0; i < src.Length; i++) s = s.Replace(src[i], rep[i]);
                return s;
            }
            return s;
        }

        private static string SostituisciLettereOmocodia(string cf)
        {
            var OmocodeChars = "LMNPQRSTUV";
            char[] cfChars = cf.ToCharArray();
            int[] pos = new[] { 6, 7, 9, 10, 12, 13, 14 };
            foreach (int i in pos) if (!Char.IsNumber(cfChars[i])) cfChars[i] = OmocodeChars.IndexOf(cfChars[i]).ToString()[0];
            return new string(cfChars);
        }

    }
}