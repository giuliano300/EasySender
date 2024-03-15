using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ControlloMittenteInsert
    {
        public SenderInsertDto sender { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloMittenteInsert()
        {
            sender = new SenderInsertDto();
            Valido = true;
            Errore = "";
        }
    }
}