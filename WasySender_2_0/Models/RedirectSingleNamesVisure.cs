using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class RedirectSingleNamesVisure
    {
        public int ListId { get; set; }
        public int Sender { get; set; }
        public int SenderAR { get; set; }
        public string TipoDocumento { get; set; }
        public string CodiceDocumento { get; set; }
        public string RicevutaRitorno { get; set; }
        public bool Valido { get; set; }
    }
}