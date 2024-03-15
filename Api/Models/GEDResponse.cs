using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GEDResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string url { get; set; }

        public GEDResponse()
        {
            success = false;
            message = "errore generico nella richiesta";
            url = null;
        }
    }
}