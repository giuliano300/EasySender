using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetRecipent
    {
        public NamesDto recipient { get; set; }
        public BulletinsDtos bulletin { get; set; }

        public GetRecipent()
        {
            recipient = new NamesDto();
            bulletin = null;
        }
    }
}