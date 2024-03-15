using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class SenderRecipients
    {
        public Sender sender { get; set; }
        public Sender senderAR { get; set; }
        public List<GetRecipent> recipients { get; set; }

        public string csvFile { get; set; }
        public SenderRecipients()
        {
            sender = null;
            senderAR = null;
            recipients = null;
            csvFile = null;
        }
    }
}