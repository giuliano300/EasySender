using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
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
        public string IBAN { get; set; }
        public int bulletinType { get; set; }
        public int namesListsId { get; set; }

    }
}