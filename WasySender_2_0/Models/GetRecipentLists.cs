using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class GetRecipentLists
    {
        public List<NamesLists> recipient { get; set; }
        public List<Bulletins> bulletin { get; set; }

        public GetRecipentLists()
        {
            recipient = new List<NamesLists>();
            bulletin = null;
        }
    }
}