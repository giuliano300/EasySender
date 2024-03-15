using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class DmProductsDto
    {        
        public int id { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
        public string productDetails { get; set; }
        public bool active { get; set; }
    }
}