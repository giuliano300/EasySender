using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ControlloDestinatario
    {
        public NamesDto Destinatario { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloDestinatario()
        {
            Destinatario = new NamesDto();
            Valido = true;
            Errore = "";
        }
    }
}