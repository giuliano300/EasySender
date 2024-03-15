using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Api.Models
{
    public class GetTaskOperationResponse
    {
        public int operationId { get; set; }
        public int state { get; set; }
        public string message { get; set; }
        public List<Task<GetSubmitResponse>> ListGetSubmitResponse { get; set; }
    }
}