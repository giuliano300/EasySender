using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetDm
    {
        public int dmId { get; set; }
        public DateTime dmDate { get; set; }

        public string product { get; set; }
        public int productId { get; set; }

        public string recipientsTypePublicName { get; set; }
        public string recipientsType { get; set; }
        public int recipientsTypeId { get; set; }

        public bool haveCreativity { get; set; }
        public int numberOfNames { get; set; }

        public decimal netPrice { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }

        public int paymentMethod { get; set; }

        public bool paid { get; set; }
        public bool complete { get; set; }

        public List<NamesDmDto> recipients { get; set; }
    }
}