using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class FatturatoTuttiIMesi
    {
        public Dictionary<int, decimal> FatturatoTuttiIMesiEasysender { get; set; }
        public Dictionary<int, decimal> FatturatoTuttiIMesiPartner { get; set; }
    }
}