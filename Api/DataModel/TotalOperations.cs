using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.DataModel
{
    public class TotalOperations
    {
        public int count { get; set; }
        public IEnumerable<GetOperations> getOperations { get; set; }
    }
}