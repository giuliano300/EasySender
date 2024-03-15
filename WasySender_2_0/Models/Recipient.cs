using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class Recipient
    {
        public int id { get; set; }
        public string businessName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string state { get; set; }
        public string fileName { get; set; }
        public byte[] attachedFile { get; set; }
        public DateTime insertDate { get; set; }
        public int currentState { get; set; }
        public bool valid { get; set; }
        public int operationId { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string codice { get; set; }
        public string dug { get; set; }
        public string houseNumber { get; set; }
        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public string fiscalCode { get; set; }
        public string mobile { get; set; }
        public decimal price { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }

        public DateTime? presaInCaricoDate { get; set; } = DateTime.Now.AddMinutes(2);
        public DateTime? consegnatoDate { get; set; } = null;

        public string stato { get; set; }
        public string NREA { get; set; }
        public int? codiceDocumento { get; set; }
        public int? tipoDocumento { get; set; }
        //PACCHI
        public string product { get; set; }
        public Nullable<DateTime> shipmentDate { get; set; }
        public int? weight { get; set; }
        public int? height { get; set; }
        public int? length { get; set; }
        public int? width { get; set; }
        public string contentText { get; set; }
        public string pathUrl { get; set; }
        public string additionalServices { get; set; }

        public bool? senderFromContract { get; set; }
        public string contrassegno { get; set; }
        public string ritornoAlMittente { get; set; }
        public string assicurazione { get; set; }

        //END PACCHI

        //SMS
        public bool? sms { get; set; }
        public string testoSms { get; set; }
        public string logo { get; set; }

    }
}