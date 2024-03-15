using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class SenderOperations
    {
        public int operationId { get; set; }
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
        public int userId { get; set; }

        public bool? AR { get; set; }
        public bool? temporary { get; set; }
        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public string telefono { get; set; }
        public string email { get; set; }

        public SenderOperations()
        {
            id = 0;
            operationId = 0;
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
            userId = 0;
            temporary = false;
            AR = false;
            complementAddress = "";
            complementNames = "";
            telefono = "";
            email = "";
        }
    }
}