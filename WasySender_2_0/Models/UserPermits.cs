using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class UserPermits
    {
        public int id { get; set; }
        public int userId { get; set; }
        public bool rol { get; set; }
        public bool lol { get; set; }
        public bool tol { get; set; }
        public bool col { get; set; }
        public bool mol { get; set; }
        public bool pec { get; set; }
        public bool pacchi { get; set; }
        public bool sms { get; set; }
        public bool marketing { get; set; }

        public UserPermits()
        {
            id = 0;
            userId = 0;
            rol = false;
            lol = false;
            tol = false;
            col = false;
            mol = false;
            pec = false;
            pacchi = false;
            sms = false;
            marketing = false;
        }
    }
}