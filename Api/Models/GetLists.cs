using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetLists
    {
        public int id { get; set; }
        public DateTime date { get; set; }

        public string name { get; set; }
        public string description { get; set; }

        public List<NamesListsDto> recipients { get; set; }
    }
}