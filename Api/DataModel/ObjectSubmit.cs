using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ObjectSubmit
    {
        public SenderDto sender { get; set; }
        public SenderDto senderAR { get; set; }
        public List<GetRecipent> recipients { get; set; }

        public string csvFile { get; set; }
        public ObjectSubmit()
        {
            sender = null;
            senderAR = null;
            recipients = null;
            csvFile = null;

        }

    }
}