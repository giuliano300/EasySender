﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.DataModel.SubmitRolResponse
{
    public class GetSubmitResponse
    {
        public int state { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string code { get; set; }
        public Prices prices { get; set; }
    }
}