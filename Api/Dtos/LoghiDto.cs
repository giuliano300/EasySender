using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class LoghiDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string logo { get; set; }
        public int userId { get; set; }
        public Nullable<int> parentUserId { get; set; }
    }
}