using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class ListiniDto
    {
        public int id { get; set; }
        public int productId { get; set; }
        public decimal creativita { get; set; }
        public decimal distrubuzione { get; set; }
        public decimal liste { get; set; }
        public decimal stampa500 { get; set; }
        public decimal stampa1000 { get; set; }
        public decimal stampa2000 { get; set; }
        public decimal stampa3000 { get; set; }
        public decimal stampa4000 { get; set; }
        public decimal stampa5000 { get; set; }

    }
}