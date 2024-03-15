using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackOffice.Models
{
    public class ModAndSendNames
    {
        public string businessName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string cap { get; set; }
        public string state { get; set; }
        public int id { get; set; }

        public bool fr { get; set; }
        public bool bn { get; set; }
        public bool rr { get; set; }

        public string tipoLettera { get; set; }
        public Sender sender { get; set; }

        public Sender senderAR { get; set; }
    }
}