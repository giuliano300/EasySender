using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetOperationReport
    {
        public string codice { get; set; }
        public string requestId { get; set; }
        public int id { get; set; }
        public string DataInserimento { get; set; }
        public string Nominativo { get; set; }
        public string complementNames { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        public string Cap { get; set; }
        public string Provincia { get; set; }
        public string Mittente { get; set; }
        public string DugMittente { get; set; }
        public string IndirizzoMittente { get; set; }
        public string CittaMittente { get; set; }
        public string CapMittente { get; set; }
        public string ProvinciaMittente { get; set; }
        public string totalPrice { get; set; }
        public string fileName { get; set; }
        public string stato { get; set; }
        public string pathGEDUrl { get; set; }
        public string consegnatoDate { get; set; }
        public int operationType { get; set; }
    }
}