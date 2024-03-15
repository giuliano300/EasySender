using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WasySender_2_0.Models
{
    public partial class operationFeatures
    {
        public int id { get; set; }
        public int operationId { get; set; }
        public string featureType { get; set; }
        public string featureValue { get; set; }
        }
}
