﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetOperationNames
    {
        public int operationId { get; set; }
        public int type { get; set; }
        public DateTime operationDate { get; set; }
        public string operationType { get; set; }
        public List<operationFeatures> operationFeatures { get; set; }
        public Sender sender { get; set; }
        public GetRecipentNew recipient { get; set; }
        public decimal price { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }
    }
}