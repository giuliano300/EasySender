using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class SenderRecipientsTelegram
    {
        public Sender sender { get; set; }
        public List<NamesTelegram> recipients { get; set; }
        public string  testo { get; set; }
    }
}