using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class MolColResponse
    {
        public string idLotto { get; set; }
        public int numeroDiInvii { get; set; }
        public string messaggio { get; set; }
        public List<string> codici { get; set; }

        public MolColResponse()
        {
            idLotto = null;
            numeroDiInvii = 0;
            messaggio = "Caricamento lotto riuscito";
            List<string> codici = null;
        }
    }
}