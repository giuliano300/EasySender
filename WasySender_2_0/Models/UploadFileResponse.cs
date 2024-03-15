using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class UploadFileResponse
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public string fileName { get; set; }
        public UploadFileResponse()
        {
            success = false;
            errorMessage = "";
            fileName = null;
        }
    }
}