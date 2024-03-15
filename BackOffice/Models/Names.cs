using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BackOffice.Models
{
    public class Names
    {
        public int id { get; set; }
        public string businessName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public string dug { get; set; }
        public string address { get; set; }
        public string houseNumber { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string state { get; set; }
        public System.DateTime insertDate { get; set; }
        public int currentState { get; set; }
        public bool valid { get; set; }
        public int operationId { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string codice { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> vatPrice { get; set; }
        public Nullable<decimal> totalPrice { get; set; }
        public byte[] attachedFile { get; set; }
        public string fileName { get; set; }
        public Nullable<System.DateTime> presaInCaricoDate { get; set; }
        public Nullable<System.DateTime> consegnatoDate { get; set; }
        public string stato { get; set; }
        public string guidUser { get; set; }
        public Nullable<bool> tipoStampa { get; set; }
        public Nullable<bool> fronteRetro { get; set; }
        public Nullable<bool> ricevutaRitorno { get; set; }
        public bool locked { get; set; }
        public Nullable<System.Guid> reSendGuid { get; set; }
        public string fiscalCode { get; set; }
        public string mobile { get; set; }
        public int namePriority { get; set; }
        public string tipoLettera { get; set; }
        public Nullable<int> IndiceNelLotto { get; set; }
        public string idLotto { get; set; }
        public string pathRecoveryFile { get; set; }
        public Nullable<bool> finalState { get; set; }
        public int? NREA { get; set; }
        public string codiceChiusura { get; set; }
        public int? codiceDocumento { get; set; }
        public int? tipoDocumento { get; set; }
        public string pathGEDUrl { get; set; }
    }
}
