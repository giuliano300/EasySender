using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Api.Dtos;

namespace Api.DataModel
{
    public class GetUserAndPermissions
    {
        public UsersDto user { get; set; }
        public UserPermitsDto permits { get; set; }

        public bool all { get; set; }
    }
}