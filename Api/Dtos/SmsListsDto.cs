using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class SmsListsDto
    {
        public int id { get; set; }
        public int valueMin { get; set; }
        public int valueMax { get; set; }
        public decimal price { get; set; }
    }
}