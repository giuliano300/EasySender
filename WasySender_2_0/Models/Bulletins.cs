using System;

namespace WasySender_2_0.Models
{    
    public class Bulletins
    {
        public int id { get; set; }
        public int namesId { get; set; }
        public string NumeroContoCorrente { get; set; }
        public string IntestatoA { get; set; }
        public string FormatoStampa { get; set; }
        public string Template { get; set; }
        public string AdditionalInfo { get; set; }
        public string IBAN { get; set; }
        public string CodiceCliente { get; set; }
        public decimal ImportoEuro { get; set; }
        public string EseguitoDaNominativo { get; set; }
        public string EseguitoDaIndirizzo { get; set; }
        public string EseguitoDaCAP { get; set; }
        public string EseguitoDaLocalita { get; set; }
        public string Causale { get; set; }
        public int BulletinType { get; set; }
        public int namesListsId { get; set; }
        public string Scadenza { get; set; }
        public string CBILL { get; set; }
        public bool PagoPA { get; set; }
        public bool? Pagato { get; set; }
        public bool? Controllato { get; set; }
        public string CodiciAvvisi { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}
