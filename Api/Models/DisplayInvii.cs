using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class DisplayInvii
    {
        public DateTime insertDate { get; set; }
        public string formato { get; set; }
        public string businessName { get; set; }
        public string namesComplete { get; set; }
        public string fileName { get; set; }
        public string valid { get; set; }
        public string currentState { get; set; }
        public string stato { get; set; }
        public string codice { get; set; }
        public string dataConsegna { get; set; }
        public string type { get; set; }
        public string options { get; set; }
        public string csv { get; set; }
        public string requestId { get; set; }
        public int id { get; set; }
        public bool notificato { get; set; }

        public DisplayInvii()
        {
            insertDate = DateTime.Now;
            businessName = "";
            formato = "";
            namesComplete = "";
            valid = "SI";
            currentState = "In Attesa";
            stato = "";
            codice = "";
            type = "";
            csv = "";
            options = "";
            dataConsegna = "";
            fileName = "";
            id = 0;
            requestId = "";
            notificato = false;
        }
    }
}