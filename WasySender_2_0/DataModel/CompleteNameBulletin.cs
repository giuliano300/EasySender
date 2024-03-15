using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class CompleteNameBulletin
    {
        public Names name { get; set; }
        public Bulletins bulletin { get; set; }
    }
}