using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ConfigurationPecUsers
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int pecTypeId { get; set; }

        public ConfigurationPecUsers()
        {
            id = 0;
            userId = 0;
            username = "";
            password = "";
            pecTypeId = 0;
        }
    }

}