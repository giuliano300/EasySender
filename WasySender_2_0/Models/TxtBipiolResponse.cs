using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class TxtBipiolResponse
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public string name { get; set; }
        public string pathTxt { get; set; }
        public string bulletinsFile { get; set; }

        public TxtBipiolResponse()
        {
            success = false;
            errorMessage = null;
            name = null;
            pathTxt = null;
            bulletinsFile = null;
        }
    }
}