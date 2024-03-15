using Api.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ObjectSubmit
    {
        public SenderDto sender { get; set; }
        public List<GetRecipent> recipients { get; set; }
    }
}