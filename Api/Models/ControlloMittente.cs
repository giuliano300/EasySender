using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ControlloMittente
    {
        public SenderDto sender { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloMittente()
        {
            sender = new SenderDto();
            Valido = true;
            Errore = "";
        }
    }
}