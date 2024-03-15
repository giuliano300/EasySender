using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Totals
    {
        public double ImportoNetto { get; set; }
        public double ImportoIva { get; set; }
        public double ImportoTotale { get; set; }
    }
}