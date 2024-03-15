using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class LogsComplete
    {
        public LogsDto log { get; set; }
        public UsersDto user { get; set; }

        public string logType { get; set; }
    }
}