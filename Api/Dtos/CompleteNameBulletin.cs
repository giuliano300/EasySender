using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class CompleteNameBulletin
    {
        public NamesDto name { get; set; }
        public BulletinsDto bulletin { get; set; }
    }
}