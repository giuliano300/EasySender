using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ResponseMOLCOL
    {
        public int? userId { get; set; }
        public bool valid { get; set; }
        public string state { get; set; }
        public string requestId { get; set; }
        public string code { get; set; }
        public byte[] docImage { get; set; }

        public ResponseMOLCOL()
        {
            userId = null;
            valid = false;
            state = "Errore interno del server";
            requestId = null;
            code = null;
            docImage = null;
        }
    }
}