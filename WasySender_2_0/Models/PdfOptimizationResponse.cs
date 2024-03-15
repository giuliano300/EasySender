using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class PdfOptimizationResponse
    {
        public string pathFile { get; set; }
        public bool success { get; set; }
        public string errorMessage { get; set; }

        public PdfOptimizationResponse()
        {
            pathFile = "";
            success = true;
            errorMessage = "";
        }
    }
}