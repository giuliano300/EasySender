using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class UploadFileRecipientResponse
    {
        public bool success { get; set; }
        public string errorMessage { get; set; }
        public Recipient name { get; set; }

        public UploadFileRecipientResponse()
        {
            success = false;
            errorMessage = "";
            name = null;
        }
    }
}