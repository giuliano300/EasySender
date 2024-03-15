using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ObjectSubmitTelegram
    {
        public SenderInsertDto sender { get; set; }
        public NamesTelegramDto recipient { get; set; }
        public string testo { get; set; }
        public bool ricevutaRitorno { get; set; }

        public ObjectSubmitTelegram()
        {
            sender = null;
            recipient = null;
            testo = "";
        }
    }
}