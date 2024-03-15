using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Response
    {
        public int? userId { get; set; }
        public bool valid { get; set; }
        public string state { get; set; }
        public string requestId { get; set; }
        public string guidUser { get; set; }
        public string code { get; set; }
        public byte[] docImage { get; set; }

        public Prices prices { get; set; }

        public Response()
        {
            userId = null;
            valid = false;
            state = "Errore interno del server";
            requestId = null;
            guidUser = null;
            code = null;
            docImage = null;
            prices = null;
        }
    }
}