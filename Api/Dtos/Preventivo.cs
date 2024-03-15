using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class Preventivo
    {
        public decimal stampa { get; set; }
        public decimal creativita { get; set; }
        public decimal lista { get; set; }
        public decimal distribuzione { get; set; }
        public decimal totale { get; set; }
    }
}