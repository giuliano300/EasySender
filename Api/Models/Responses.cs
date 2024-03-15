using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Responses
    {
        public string dataEsito { get; set; }
        public string stato { get; set; }

        public Responses()
        {
            dataEsito = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            stato = "";
        }
    }
}