using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class DisplayCounters
    {
        public string businessName { get; set; }
        public int totalGed { get; set; }
        public string posteGed { get; set; }
        public int numberOfNames { get; set; }
        public decimal totalPrice { get; set; }

        public DisplayCounters()
        {
            businessName = "";
            numberOfNames = 0;
            totalPrice = 0;
        }
    }
}