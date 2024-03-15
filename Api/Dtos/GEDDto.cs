using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class GEDDto
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public int userId { get; set; }
        public string pathFile { get; set; }

    }
}