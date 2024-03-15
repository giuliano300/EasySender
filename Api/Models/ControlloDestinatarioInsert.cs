using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ControlloDestinatarioInsert
    {
        public NameInsertDto Destinatario { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloDestinatarioInsert()
        {
            Destinatario = new NameInsertDto();
            Valido = true;
            Errore = "";
        }
    }
}