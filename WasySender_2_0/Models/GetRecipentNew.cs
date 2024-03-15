using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class GetRecipentNew
    {
        public Names recipient { get; set; }
        public Bulletins bulletin { get; set; }

        public GetRecipentNew()
        {
            recipient = null;
            bulletin = null;
        }
    }
}