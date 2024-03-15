using BackOffice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackOffice.ViewModel
{
    public class GetNamesComplete
    {
        public Sender sender { get; set; }
        public Sender senderAR { get; set; }
        public Names name { get; set; }
        public Operations operation { get; set; }
    }
}