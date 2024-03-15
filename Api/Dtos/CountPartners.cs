using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class CountPartners
    {
        public int daySends { get; set; }
        public int weekSends { get; set; }
        public int monthSends { get; set; }
        public decimal fatturatoPartner { get; set; }
        public decimal fatturatoEasysender { get; set; }
        public int totalYearSends { get; set; }
        public int totalYearPartnerSends { get; set; }

        public CountPartners()
        {
            fatturatoPartner = 0;
            fatturatoEasysender = 0;
            totalYearPartnerSends = 0;
            totalYearSends = 0;
            daySends = 0;
            weekSends = 0;
            monthSends = 0;
        }
    }
}