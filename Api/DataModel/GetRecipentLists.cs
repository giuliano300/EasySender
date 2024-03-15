using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetRecipentLists
    {
        public List<NamesListsDto> recipient { get; set; }
        public List<BulletinsDtos> bulletin { get; set; }

        public GetRecipentLists()
        {
            recipient = new List<NamesListsDto>();
            bulletin = null;
        }
    }
}