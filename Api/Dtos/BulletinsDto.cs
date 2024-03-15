using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class BulletinsDto
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
        public bool? PagoPA { get; set; }
        public bool? Pagato { get; set; }
        public bool? Controllato { get; set; }
        public string CodiciAvvisi { get; set; }
        public DateTime? DataPagamento { get; set; }

        public BulletinsDto()
        {
            id = 0;
            namesId = 0;
            NumeroContoCorrente = "";
            IntestatoA = "";
            FormatoStampa = "";
            Template = "";
            AdditionalInfo = "";
            IBAN = "";
            CodiceCliente = "";
            ImportoEuro =  0;
            EseguitoDaNominativo = "";
            EseguitoDaIndirizzo = "";
            EseguitoDaCAP = "";
            EseguitoDaLocalita = "";
            Causale = "";
            BulletinType = 0;
            namesListsId = 0;
            Scadenza = "";
            CBILL = "";
            PagoPA = false;
            Pagato = false;
            Controllato = false;
            CodiciAvvisi = "";
            DataPagamento = null;
        }
    }
}