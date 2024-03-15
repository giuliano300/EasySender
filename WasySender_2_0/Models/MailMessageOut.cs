using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class MailMessageOut
    {
        public string from { get; set; }
        public string to { get; set; }
        public string cc { get; set; }
        public string subject { get; set; }
        public string msg { get; set; }
        public List<string> attachments { get; set; }
        public string id { get; set; }
        public DateTime date { get; set; }
        public string status { get; set; }
        public int index { get; set; }
        public bool fromDatabase { get; set; }
    }
}