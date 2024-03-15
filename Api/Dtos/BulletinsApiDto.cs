using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class BulletinsApiDto
    {
        public string NumeroContoCorrente { get; set; }
        public string IntestatoA { get; set; }
        public string IBAN { get; set; }
        public string CodiceCliente { get; set; }
        public decimal ImportoEuro { get; set; }
        public string EseguitoDaNominativo { get; set; }
        public string EseguitoDaIndirizzo { get; set; }
        public string EseguitoDaCAP { get; set; }
        public string EseguitoDaLocalita { get; set; }
        public string Causale { get; set; }
    }
}