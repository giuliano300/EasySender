using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetRecipent
    {
        public Names recipient { get; set; }
        public BulletinsDtos bulletin { get; set; }

        public GetRecipent()
        {
            recipient = new Names();
            bulletin = null;
        }
    }
}