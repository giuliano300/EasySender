using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class DmConfiguration
    {
        public int? id { get; set; }
        public int? dmId { get; set; }
        public string DiCosaSiOccupaLAzienda { get; set; }
        public string ObiettivoPrincipale { get; set; }
        public string Target { get; set; }
        public string ElementiPrincipali { get; set; }
        public string Logo { get; set; }
        public string sessionUser { get; set; }

        public DmConfiguration()
        {
            id = null;
            dmId = null;
            DiCosaSiOccupaLAzienda = "";
            ObiettivoPrincipale = "";
            Target = "";
            ElementiPrincipali = "";
            Logo = "";
            sessionUser = HttpContext.Current.Session.SessionID;
        }
    }
}