using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class PecEmailSend
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string fromEmail { get; set; }
        public string toEmail { get; set; }
        public string ccEmail { get; set; }
        public string @object { get; set; }
        public string body { get; set; }
        public string attachment { get; set; }
        public string attachmentName { get; set; }
        public int PecTypeId { get; set; }
        public DateTime date { get; set; }
    }
}