using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ControlloDestinatario
    {
        public NamesLists Destinatario { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloDestinatario()
        {
            Destinatario = new NamesLists();
            Valido = true;
            Errore = "";
        }
    }
}