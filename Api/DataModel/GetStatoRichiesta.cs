using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataModel
{
    public class GetStatoRichiesta
    {
        public string requestId { get; set; }
        public string statoDescrizione { get; set; }
        public string numeroServizio { get; set; }
        public string dataEsito { get; set; }

        public GetStatoRichiesta()
        {
            requestId = null;
            statoDescrizione = "";
            numeroServizio = "";
            dataEsito = "";
        }

    }
}