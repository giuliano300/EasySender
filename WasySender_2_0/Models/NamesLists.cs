using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class NamesLists
    {
        public int id { get; set; }
        public string businessName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }

        [Required]
        public string dug { get; set; }

        [Required]
        public string address { get; set; }

        public string houseNumber { get; set; }

        [Required]
        public string cap { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string province { get; set; }

        [Required]
        public string state { get; set; }

        [Required]
        public int listId { get; set; }

        public string fileName { get; set; }

        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public string fiscalCode { get; set; }
        public string mobile { get; set; }

        public bool valid { get; set; }
        public string errorMessage { get; set; }
        public string NREA { get; set; }
        public bool? noUse { get; set; }
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
        public string logo { get; set; }


        public NamesLists()
        {
            id = 0;
            name = "";
            surname = "";
            businessName = "";
            dug = "";
            address = "";
            houseNumber = "";
            cap = "";
            city = "";
            province = "";
            state = "";
            listId = 0;
            fileName = "";
            complementAddress = "";
            complementNames = "";
            fiscalCode = "";
            mobile = "";
            valid = true;
            noUse = false;
            errorMessage = "";
            NREA = "";
            codiceDocumento = null;
            tipoDocumento = null;
            product = "";
            shipmentDate = null;
            senderFromContract = false;
            contrassegno = "";
            ritornoAlMittente = "";
            assicurazione = "";

            sms = false;
            testoSms = null;

            logo = null;
        }

    }
}