using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.DataModel;
using WasySender_2_0.Models;

namespace WasySender_2_0.ViewModel
{
    public class SmsViewModel
    {
        public SmsUsers sms { get; set; }
        public Users user { get; set; }
    }
}