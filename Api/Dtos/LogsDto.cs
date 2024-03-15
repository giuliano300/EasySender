using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Api.Controllers.Api.LogsController;

namespace Api.Dtos
{
    public class LogsDto
    {
        public long id { get; set; }
        public System.DateTime date { get; set; }
        public int logType { get; set; }
        public int userId { get; set; }

        public string description { get; set; }

        public LogsDto()
        {
            date = DateTime.Now;
            logType = (int)LogType.login;
        }

    }
}