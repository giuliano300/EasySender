using Api.ServiceMOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ResponseMOLConfirm
    {
        public EsitoPostaEvo EsitoPostaEvo { get; set; }
        public string NumeroRaccomandata { get; set; }
        public DateTime? DataAccettazione { get; set; }

        public ResponseMOLConfirm()
        {
            EsitoPostaEvo = EsitoPostaEvo.KO;
            NumeroRaccomandata = null;
            DataAccettazione = null;
        }
    }
}