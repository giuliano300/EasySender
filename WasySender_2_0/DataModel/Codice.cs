using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.DataModel
{
    public class Codice
    {
        public string dataValidita { get; set; }
        public DateTime dataPagamento { get; set; }
        public string tipo { get; set; }
        public string codice { get; set; }
        public string codiceCliente { get; set; }
        public string valuta { get; set; }
        public string prezzo { get; set; }
    }
}