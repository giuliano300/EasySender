using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetOperationResponseRolLol
    {
        public int operationId { get; set; }
        public int state { get; set; }
        public string message { get; set; }
        public List<GetSubmitResponses> ListGetSubmitResponse { get; set; }
    }
}