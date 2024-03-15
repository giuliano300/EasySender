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
        public string NumeroRaccomandata { get; set; }
        public string DescrizioneEsito { get; set; }
        public DateTime? DataAccettazione { get; set; }

        public ResponseCOLConfirm()
        {
            EsitoPostaEvo = EsitoPostaEvo.KO;
            NumeroRaccomandata = null;
            DataAccettazione = null;
            DescrizioneEsito = "";
        }
    }
}