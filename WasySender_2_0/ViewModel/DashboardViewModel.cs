using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.ViewModel
{
    public class DashboardViewModel
    {
        public Users user { get; set; }
        public List<Notifications> Notifications { get; set; }
        public List<Notifications> Communication { get; set; }
    }
}