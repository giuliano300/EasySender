using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataModel
{
    public class GetRecipentNew
    {
        public NamesDtos recipient { get; set; }
        public BulletinsDtos bulletin { get; set; }

        public GetRecipentNew()
        {
            recipient = new NamesDtos();
            bulletin = null;
        }
    }
}