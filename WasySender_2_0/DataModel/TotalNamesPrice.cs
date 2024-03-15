using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.DataModel
{
    public class TotalNamesPrice
    {
        public decimal importoNetto { get; set; }
        public decimal importoIva { get; set; }
        public decimal importoTotale { get; set; }
        public int numberOfNames { get; set; }
        public int operationId { get; set; }
    }
}