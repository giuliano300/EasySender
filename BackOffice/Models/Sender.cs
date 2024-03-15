using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Models
{
    public class Sender
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

        public string complementNames { get; set; }

        public string complementAddress { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }

        public bool AR { get; set; }

    }
}