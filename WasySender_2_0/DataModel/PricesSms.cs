using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.DataModel
{
    public class PricesSms
    {
        public decimal untiPrice { get; set; }
        public decimal netPrice { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }
    }
}