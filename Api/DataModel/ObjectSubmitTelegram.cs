using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ObjectSubmitTelegram
    {
        public SenderDto sender { get; set; }
        public List<NamesTelegramDto> recipients { get; set; }
        public string testo { get; set; }
    }
}