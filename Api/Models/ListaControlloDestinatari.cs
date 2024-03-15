using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ListaControlloDestinatari
    {
        public List<ControlloDestinatario> ListCrtlD { get; set; }
        public bool ItaliaEstero { get; set; }
        public string ErroreItaliaEstero { get; set; }

        public ListaControlloDestinatari()
        {
            ListCrtlD = new List<ControlloDestinatario>();
            ItaliaEstero = false;
            ErroreItaliaEstero = "";
        }

    }
}