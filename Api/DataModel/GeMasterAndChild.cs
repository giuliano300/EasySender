using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Api.Dtos;

namespace Api.DataModel
{
    public class GeMasterAndChild
    {
        public UsersDto user { get; set; }
        public List<UsersDto> child { get; set; }
    }
}