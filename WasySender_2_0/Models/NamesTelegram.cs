using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class NamesTelegram
    {
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
    }
}