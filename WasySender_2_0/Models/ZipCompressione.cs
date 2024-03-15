using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ZipCompressione
    {
        public bool success { get; set; }
        public string pathCompressedFile { get; set; }
        public string erroMessage { get; set; }

        public ZipCompressione()
        {
            success = false;
            pathCompressedFile = "";
            erroMessage = "";
        }
    }
}