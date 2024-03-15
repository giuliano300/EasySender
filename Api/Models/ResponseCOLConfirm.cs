using Api.ServiceCOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class ResponseCOLConfirm
    {
        public EsitoPostaEvo EsitoPostaEvo { get; set; }
        public string NumeroLettera { get; set; }
        public DateTime? DataAccettazione { get; set; }

        public ResponseCOLConfirm()
        {
            EsitoPostaEvo = EsitoPostaEvo.KO;
            NumeroLettera = null;
            DataAccettazione = null;
        }
    }
}