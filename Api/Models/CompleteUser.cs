using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class CompleteUser
    {
        public PropertyDto property { get; set; }
        public UsersDto user { get; set; }

        public int? subUsers { get; set; }

        public CompleteUser()
        {
            subUsers = 0;
        }
    }
}