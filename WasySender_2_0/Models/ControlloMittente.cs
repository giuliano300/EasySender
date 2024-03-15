using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ControlloMittente
    {
        public Sender sender { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloMittente()
        {
            sender = new Sender();
            Valido = true;
            Errore = "";
        }
    }
}