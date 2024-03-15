using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetUserAndPermissions
    {
        public Users user { get; set; }
        public UserPermits permits { get; set; }

        public bool all { get; set; }
    }
}