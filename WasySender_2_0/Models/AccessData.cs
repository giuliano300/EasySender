using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Oauth0.Models
{
    public class AccessData
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string nickname { get; set; }
        public string updatetd { get; set; }
        public string expiredate { get; set; }
        public string token { get; set; }
    }
}