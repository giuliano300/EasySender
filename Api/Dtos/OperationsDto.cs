using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class OperationsDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public int userId { get; set; }
        public int? userParentId { get; set; }
        public System.DateTime date { get; set; }
        public int operationType { get; set; }
        public bool demoOperation { get; set; }
        public bool complete { get; set; }
        public bool areaTestOperation { get; set; }
    }
}