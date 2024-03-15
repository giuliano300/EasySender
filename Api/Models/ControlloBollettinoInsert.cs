using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ControlloBollettinoInsert
    {
        public BulletinInsertDto Bollettino { get; set; }

        public bool Valido { get; set; }

        public string Errore { get; set; }

        public ControlloBollettinoInsert()
        {
            Bollettino = new BulletinInsertDto();
            Valido = true;
            Errore = "";
        }
    }
}