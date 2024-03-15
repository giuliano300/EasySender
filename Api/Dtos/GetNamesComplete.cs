using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class GetNamesComplete
    {
        public SenderDto sender { get; set; }
        public SenderDto senderAR { get; set; }
        public NamesDtos name { get; set; }
        public OperationsDto operation { get; set; }
    }
}