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

        public DateTime? presaInCaricoDate { get; set; }
        public DateTime? consegnatoDate { get; set; }

        public string guidUser { get; set; }
        public string stato { get; set; }

        public bool tipoStampa { get; set; }
        public bool fronteRetro { get; set; }
        public bool ricevutaRitorno { get; set; }
        public bool locked { get; set; }

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
            attachedFile = null;
            insertDate = DateTime.Now;
            currentState = 0;
            valid = false;
            operationId = 0;
            requestId = null;
            orderId = null;
            codice = null;
            guidUser = null;
            dug = "";
            houseNumber = "";
            complementNames = "";
            complementAddress = "";
            price = 0;
            vatPrice = 0;
            totalPrice = 0;

            presaInCaricoDate = DateTime.Now.AddMinutes(2);
            consegnatoDate = null;

            tipoStampa = false;
            fronteRetro = false;
            ricevutaRitorno = false;
            stato = "";
            locked = false;
        }
    }
}