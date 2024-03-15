using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class GetDm
    {
        public int dmId { get; set; }
        public DateTime dmDate { get; set; }

        public string product { get; set; }
        public int productId { get; set; }

        public string recipientsType { get; set; }
        public int recipientsTypeId { get; set; }

        public bool haveCreativity { get; set; }
        public int numberOfNames { get; set; }

        public List<NamesDm> recipients { get; set; }
    }
}