using Api.ServiceRol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Api.Models
{
    public class GetRicevutaAccettazione
    {
        public RecuperaRicevutaAccettazioneResult RecuperaRicevutaAccettazioneResult { get; set; }
        public string file { get; set; }
    }
}