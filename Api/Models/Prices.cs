﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Prices
    {
        public decimal price { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }

        public Prices(){
            price = 0;
            vatPrice = 0;
            totalPrice = 0;
        }
   }

}