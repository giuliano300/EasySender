using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetOperationNamesNew
    {
        public string nameN { get; set; }
        public string surnameN { get; set; }
        public string businessNameN { get; set; }
        public string dugN { get; set; }
        public string addressN { get; set; }
        public string capN { get; set; }
        public string cityN { get; set; }
        public string provinceN { get; set; }
        public string nameS { get; set; }
        public string surnameS { get; set; }
        public string businessNameS { get; set; }
        public string requestId { get; set; }
        public string dugS { get; set; }
        public string addressS { get; set; }
        public string capS { get; set; }
        public string cityS { get; set; }
        public string provinceS { get; set; }
        public string fileName { get; set; }
        public string codice { get; set; }
        public string stato { get; set; }
        public bool valid { get; set; }
        public int operationType { get; set; }
        public DateTime presaInCaricoDate { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> vatPrice { get; set; }
        public Nullable<decimal> totalPrice { get; set; }
    }
}