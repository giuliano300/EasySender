using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class Request
    {
        public string requestId { get; set; }
        public string guidUser { get; set; }

        public Request()
        {
            requestId = null;
            guidUser = null;
        }
    }
}