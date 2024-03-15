using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
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
        public int dlr_ok { get; set; }
        public int dlr_ko { get; set; }
        public int dlr_wait { get; set; }
        public string message { get; set; }
        public string sender { get; set; }
        public DateTime startdate { get; set; }
        public string upperlimit { get; set; }

        public ResponseSmsStats()
        {
            errcode = "";
            extrainfo = "";
            targetcode = "";
            dlr_ok = 0;
            dlr_ko = 0;
            dlr_wait = 0;
            message = "";
            sender = "";
            startdate = DateTime.Now.AddDays(1);
            upperlimit = "";
        }
    }

}