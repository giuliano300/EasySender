using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetLists
    {
        public int id { get; set; }
        public DateTime date { get; set; }

        public string name { get; set; }
        public string description { get; set; }

        public List<Recipient> recipients { get; set; }

        public GetLists()
        {
            id = 0;
            date = DateTime.Now;
            name = "";
            description = "";
            recipients = null;
        }
    }
}