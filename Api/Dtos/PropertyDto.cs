using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class PropertyDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string logo { get; set; }

    }
}