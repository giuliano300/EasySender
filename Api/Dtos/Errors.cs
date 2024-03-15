using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Dtos
{
    public class Errors
    {
        public int daySends { get; set; }
        public int weekSends { get; set; }
        public int monthSends { get; set; }
        public int previousMonthSends { get; set; }
        public int dayErrors { get; set; }
        public int weekErrors { get; set; }
        public int monthErrors { get; set; }
        public int previousMonthErrors { get; set; }

        public Errors()
        {
            dayErrors = 0;
            weekErrors = 0;
            monthErrors = 0;
            daySends = 0;
            weekSends = 0;
            monthSends = 0;
            previousMonthSends = 0;
            previousMonthErrors = 0;
        }
    }
}