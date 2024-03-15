using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class crt
    {
        public string Errore { get; set; }
        public bool Valido { get; set; }
        public bool ItaliaEstero { get; set; }

        public crt()
        {
            Errore = "";
            Valido = true;
            ItaliaEstero = false;
        }
    }
}