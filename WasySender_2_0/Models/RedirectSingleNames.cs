using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class RedirectSingleNames
    {
        public int ListId { get; set; }
        public int Sender { get; set; }
        public int SenderAR { get; set; }
        public string Bollettini { get; set; }
        public string TipoStampa { get; set; }
        public string FronteRetro { get; set; }
        public string RicevutaRitorno { get; set; }
        public string TipoLettera { get; set; }
        public string Formato { get; set; }
        public string Logo { get; set; }
        public bool Valido { get; set; }
    }
}