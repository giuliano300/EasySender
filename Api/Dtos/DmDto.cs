using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class DmDto
    {
        public int id { get; set; }

        [Required]
        public int userId { get; set; }

        public int? userParentId { get; set; }

        [Required]
        public int productId { get; set; }

        [Required]
        public System.DateTime date { get; set; }

        [Required]
        public int recipientsType { get; set; }

        [Required]
        public bool haveCreativity { get; set; }

        [Required]
        public int numberOfNames { get; set; }

        [Required]
        public decimal netPrice { get; set; }

        [Required]
        public decimal vatPrice { get; set; }

        [Required]
        public decimal totalPrice { get; set; }

        [Required]
        public string sessionUser { get; set; }

        public int? paymentMethod { get; set; }

        [Required]
        public bool complete { get; set; }

        [Required]
        public bool paid { get; set; }
    }
}