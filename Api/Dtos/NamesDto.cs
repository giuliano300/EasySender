using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class NamesDto
    {
        public int? id { get; set; }
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
        public byte[] attachedFile { get; set; }

        public string complementNames { get; set; }
        public string complementAddress { get; set; }

        public string fileName { get; set; }

        public decimal price { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }

        public DateTime? presaInCaricoDate { get; set; }
        public DateTime? consegnatoDate { get; set; }

        public string stato { get; set; }
        public string guidUser { get; set; }

        public bool tipoStampa { get; set; }
        public bool fronteRetro { get; set; }
        public bool ricevutaRitorno { get; set; }
        public bool locked { get; set; }
    }
}