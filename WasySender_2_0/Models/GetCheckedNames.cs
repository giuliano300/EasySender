using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.DataModel.SubmitRolResponse;

namespace WasySender_2_0.Models
{
    public class GetCheckedNames
    {
        public NamesLists name { get; set; }
        public bool valid { get; set; }
        public Prices price { get; set; }
    }
}