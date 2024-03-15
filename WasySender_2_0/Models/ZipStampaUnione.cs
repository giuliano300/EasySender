using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ZipStampaUnione
    {
        public bool success { get; set; }
        public string filePath { get; set; }
        public string name { get; set; }
        public string pathCsv { get; set; }
        public string pathDoc { get; set; }
        public string errorMessage { get; set; }

        public ZipStampaUnione()
        {
            success = false;
            filePath = "";
            name = "";
            pathCsv = "";
            pathDoc = "";
            errorMessage = "";
        }
    }
}