using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class Logs
    {
        public long id { get; set; }
        public System.DateTime date { get; set; }
        public int logType { get; set; }
        public int userId { get; set; }
        public string description { get; set; }

        public Logs()
        {
            date = DateTime.Now;
            logType = (int)LogType.login;
        }

    }
}