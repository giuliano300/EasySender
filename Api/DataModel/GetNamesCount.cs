using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetNamesCount
    {
        public int totalNumber { get; set; }
        public int complete { get; set; }
        public int incomplete { get; set; }
        public int percentage { get; set; }
    }
}