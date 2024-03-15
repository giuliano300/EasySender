using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Api.Models
{
    public class ObjectSenderNameBulletin
    {
        public SenderInsertDto sender { get; set; }
        public SenderInsertDto senderAR { get; set; }
        public NameInsertDto recipient { get; set; }
        public BulletinsApiDto bulletin { get; set; }
        public int tipoStampa { get; set; }
        public int fronteRetro { get; set; }
        public int ricevutaRitorno { get; set; }

        public ObjectSenderNameBulletin()
        {
            sender = null;
            senderAR = null;
            recipient = null;
            bulletin = null;
            tipoStampa = 0;
            fronteRetro = 0;
            ricevutaRitorno = 0;
        }
    }
}