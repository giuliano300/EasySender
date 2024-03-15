using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class BulletinsDtos
    {
        public string numeroContoCorrente { get; set; }
        public string intestatoA { get; set; }
        public string codiceCliente { get; set; }
        public decimal importoEuro { get; set; }
        public string eseguitoDaNominativo { get; set; }
        public string eseguitoDaIndirizzo { get; set; }
        public string eseguitoDaCAP { get; set; }
        public string eseguitoDaLocalita { get; set; }
        public string causale { get; set; }
        public int bulletinType { get; set; }
        public int namesListsId { get; set; }
        public string Scadenza { get; set; }
        public string CBILL { get; set; }
        public bool PagoPA { get; set; }
        public string CodiciAvvisi { get; set; }
        public bool? Pagato { get; set; }
        public bool? Controllato { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}