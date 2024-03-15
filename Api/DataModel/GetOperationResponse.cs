using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetOperationResponse
    {
        public int operationId { get; set; }
        public int state { get; set; }
        public string message { get; set; }
        public List<GetSubmitResponse> ListGetSubmitResponse { get; set; }
    }
}