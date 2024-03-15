using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class SenderDtos
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string address { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string state { get; set; }
        public int userId { get; set; }
        public string businessName { get; set; }
        public string dug { get; set; }
        public string houseNumber { get; set; }
        public int operationId { get; set; }
        public bool? AR { get; set; }

    }
}