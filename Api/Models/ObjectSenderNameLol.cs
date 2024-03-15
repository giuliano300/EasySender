﻿using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ObjectSenderNameLol
    {
        public SenderInsertDto sender { get; set; }
        public NameInsertDto recipient { get; set; }
        public int tipoStampa { get; set; }
        public int fronteRetro { get; set; }
        public int tipoLettera { get; set; }

        public ObjectSenderNameLol()
        {
            sender = null;
            recipient = null;
            tipoStampa = 0;
            fronteRetro = 0;
            tipoLettera = 4;
        }
    }
}