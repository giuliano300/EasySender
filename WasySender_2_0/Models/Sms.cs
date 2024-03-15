using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class Sms
    {
        public string gender { get; set; }
        public string zip { get; set; }
        public string province { get; set; }
        public string mindob { get; set; }
        public string maxdob { get; set; }
    }

    public class ResponseSmsRequest
    {
        public string code { get; set; }
        public string quantity { get; set; }
        public string extrainfo { get; set; }
        public string errcode { get; set; }
        public string minEta { get; set; }
        public string maxEta { get; set; }
        public Sms sms { get; set; }
    }

    public class SmsSend
    {
        public string targetcode { get; set; }
        public string sender { get; set; }
        public string message { get; set; }
        public string upperlimit { get; set; }
        public DateTime startdate { get; set; }
    }

    public class ResponseSmsSend
    {
        public string extrainfo { get; set; }
        public string errcode { get; set; }
    }

    public class ResponseSmsStats
    {
        public string errcode { get; set; }
        public string extrainfo { get; set; }
        public string targetcode { get; set; }
        public string dlr_ok { get; set; }
        public string dlr_ko { get; set; }
        public string dlr_wait { get; set; }
        public string message { get; set; }
        public string sender { get; set; }
        public string startdate { get; set; }
        public string upperlimit { get; set; }
    }
}