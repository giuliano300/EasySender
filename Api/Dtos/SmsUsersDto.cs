using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class SmsUsersDto
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int? userParentId { get; set; }
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
        public DateTime date { get; set; }
        public bool paid { get; set; }
        public decimal price { get; set; }
        public decimal vat { get; set; }
        public decimal total { get; set; }
        public int? paymentMethod { get; set; }
        public bool planned { get; set; }
        public int quantity { get; set; }

        public SmsUsersDto()
        {
            id = 0;
            userId = 0;
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
            date = DateTime.Now;
            paid = false;
            price = 0;
            vat = 0;
            total = 0;
            paymentMethod = null;
            planned = false;
            quantity = 0;
            userParentId = null;
        }
    }
}