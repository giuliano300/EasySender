using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class ListsDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int userId { get; set; }
        public int? userParentId { get; set; }
        public System.DateTime date { get; set; }
        public bool temporary { get; set; }
    }
}