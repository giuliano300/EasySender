using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ControlloBollettino
    {
        public Bulletins Bollettino { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloBollettino()
        {
            Bollettino = new Bulletins();
            Valido = true;
            Errore = "";
        }
    }
}