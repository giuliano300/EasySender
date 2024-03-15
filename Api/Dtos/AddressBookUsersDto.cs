using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class AddressBookUsersDto
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string email { get; set; }
    }
}