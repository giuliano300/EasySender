using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BackOffice.Models
{
    public class EnumClass
    {
        public enum SenderPgkId : int
        {
            [Description("Poste SDA")]
            POSTE = 1,
            [Description("T.W.S.")]
            TWS = 2
        }
        public enum operationType : int
        {
            [Description("ROL")]
            ROL = 1,
            [Description("LOL")]
            LOL = 2,
            [Description("TOL")]
            TOL = 3,
            [Description("MOL")]
            MOL = 4,
            [Description("COL")]
            COL = 5,
            [Description("VOL")]
            VOL = 6
        }
    }
}