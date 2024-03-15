using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetSms
    {
        public UsersDto user { get; set; }
        public SmsUsersDto sms { get; set; }
    }
}