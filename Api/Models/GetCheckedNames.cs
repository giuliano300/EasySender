using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetCheckedNames
    {
        public NamesDtos name { get; set; }
        public bool valid { get; set; }
        public Prices price { get; set; }
    }
}