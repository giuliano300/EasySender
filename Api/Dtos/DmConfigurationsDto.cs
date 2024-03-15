using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class DmConfigurationsDto
    {
        public int id { get; set; }
        public int dmId { get; set; }
        public string DiCosaSiOccupaLAzienda { get; set; }
        public string ObiettivoPrincipale { get; set; }
        public string Target { get; set; }
        public string ElementiPrincipali { get; set; }
        public string Logo { get; set; }
        public string sessionUser { get; set; }
    }
}