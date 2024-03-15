using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetOperations
    {
        public int operationId { get; set; }
        public DateTime operationDate { get; set; }
        public string operationType { get; set; }
        public List<operationFeaturesDto> operationFeatures { get; set;}
        public SenderDto sender { get; set; }
        public List<NamesDtos> recipients { get; set; }
        public decimal price { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }
    }
}