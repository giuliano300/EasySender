using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ZipResponse
    {
        public string pathFile { get; set; }
        public bool success { get; set; }

        public ZipResponse()
        {
            pathFile = "";
            success = true;
        }
    }
}