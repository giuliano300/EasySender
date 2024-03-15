using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public enum notificationType
    {
        communication = 1,
        notification = 2
    }

    public class Notifications
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool enabled { get; set; }
        public string usersId { get; set; }
        public int notificationType { get; set; }
    }
}