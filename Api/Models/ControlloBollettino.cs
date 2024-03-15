using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ControlloBollettino
    {
        public BulletinsDtos Bollettino { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloBollettino()
        {
            Bollettino = new BulletinsDtos();
            Valido = true;
            Errore = "";
        }
    }
}