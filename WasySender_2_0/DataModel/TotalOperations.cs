using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WasySender_2_0.Models;

namespace WasySender_2_0.DataModel
{
    public class TotalOperations
    {
        public int count { get; set; }
        public IEnumerable<GetOperations> getOperations { get; set; }
    }
}