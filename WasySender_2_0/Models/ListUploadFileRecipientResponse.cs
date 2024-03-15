using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class ListUploadFileRecipientResponse
    {
        public string filePath { get; set; }
        public List<UploadFileRecipientResponse> name { get; set; }

        public ListUploadFileRecipientResponse()
        {
            filePath = "";
            name = null;
        }
    }
}