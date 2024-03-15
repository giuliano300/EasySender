using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class TemporaryValidateTableDto
    {
        public Guid id { get; set; }
        public string sessionId { get; set; }
        public int userId { get; set; }
        public bool valid { get; set; }
        public int totalNames { get; set; }
    }
}