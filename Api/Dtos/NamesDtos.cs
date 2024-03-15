using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class NamesDtos
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
        public System.DateTime insertDate { get; set; }
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
        public decimal price { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }
        public bool notificato { get; set; }

        public string pathGEDUrl { get; set; }


        public DateTime? presaInCaricoDate { get; set; }
        public DateTime? consegnatoDate { get; set; }

        public string guidUser { get; set; }
        public string stato { get; set; }

        public bool tipoStampa { get; set; }
        public bool fronteRetro { get; set; }
        public bool ricevutaRitorno { get; set; }
        public string fiscalCode { get; set; }
        public string mobile { get; set; }
        public int namePriority { get; set; }
        public string tipoLettera { get; set; }
        public string pathRecoveryFile { get; set; }
        public string email { get; set; }
        public string codiceChiusura { get; set; }

        public bool finalState { get; set; }
        public int? numberOfPages { get; set; }
        public int? NREA { get; set; }
        public Nullable<int> codiceDocumento { get; set; }
        public Nullable<int> tipoDocumento { get; set; }


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

        public string contrassegno { get; set; }
        public string ritornoAlMittente { get; set; }
        public string assicurazione { get; set; }
        public bool? senderFromContract { get; set; }
        //END PACCHI

        //SMS
        public bool? sms { get; set; }
        public string testoSms { get; set; }
        //JUST EAT
        public bool? fileScaricato { get; set; }
        public DateTime? dataDownload { get; set; }
        public int? operationType { get; set; }

        public string logo { get; set; }
        public Nullable<bool> AvvisoRicevimentoDigitale { get; set; }
        public string pec { get; set; }
        public string cron { get; set; }
        public NamesDtos()
        {
            id = 0;
            businessName = "";
            name = "";
            surname = "";
            address = "";
            cap = "";
            city = "";
            province = "";
            state = "";
            fileName = "";
            email = "";
            attachedFile = null;
            insertDate = DateTime.Now;
            currentState = 0;
            valid = false;
            operationId = 0;
            requestId = null;
            orderId = null;
            codice = null;
            guidUser = null;
            operationType = null;
            dug = "";
            houseNumber = "";
            complementNames = "";
            complementAddress = "";
            pathGEDUrl = "";
            price = 0;
            vatPrice = 0;
            totalPrice = 0;

            presaInCaricoDate = DateTime.Now.AddMinutes(2);
            consegnatoDate = null;

            tipoStampa = false;
            fronteRetro = false;
            ricevutaRitorno = false;
            notificato = false;
            stato = "";
            fiscalCode = null;
            mobile = null;
            pathRecoveryFile = null;

            namePriority = 0;

            tipoLettera = "Posta1";
            finalState = false;
            numberOfPages = 0;
            NREA = null;
            codiceDocumento = null;
            tipoDocumento = null;
            codiceChiusura = null;

            product = "";
            shipmentDate = DateTime.Now;
            weight = 0;
            height = 0;
            length = 0;
            width = 0;
            contentText = "";
            pathUrl = "";
            additionalServices = null;
            senderFromContract = false;

            contrassegno = "";
            ritornoAlMittente = "";
            assicurazione = "";

            sms = false;
            testoSms = null;
            logo = null;

            fileScaricato = false;
            dataDownload = null;
        }
    }
}