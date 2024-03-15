using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataModel
{
    public class GetBill
    {
        public string requestId { get; set; }
        public string prodotto { get; set; }
        public string mittente { get; set; }
        public string operatore { get; set; }
        public int operationId { get; set; }
        public string destinatario { get; set; }
        public string completamentoNomeDestinatario { get; set; }
        public string indirizzoDestinatario { get; set; }
        public string completamentoIndirizzoDestinatario { get; set; }
        public string cap { get; set; }
        public string citta { get; set; }
        public string provincia { get; set; }
        public string codice { get; set; }
        public string stato { get; set; }
        public DateTime DataInserimento { get; set; }

    }
}