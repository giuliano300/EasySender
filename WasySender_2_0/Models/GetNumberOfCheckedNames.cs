using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public class GetNumberOfCheckedNames
    {
        public List<GetCheckedNames> checkedNames { get; set; }
        public int numberOfValidNames { get; set; }
        public int operationId { get; set; }
        public string state { get; set; }
        public bool valid { get; set; }

        public GetNumberOfCheckedNames()
        {
            checkedNames = null;
            numberOfValidNames = 0;
            state = null;
            operationId = 0;
            valid = false;
        }
    }
}