using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.DataModel
{
    public class Body
    {
        public string countryCode { get; set; }
        public string vatNumber { get; set; }
        public string requestDate { get; set; }
        public bool valid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }
}