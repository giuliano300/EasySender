//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class DmConfigurations
    {
        public int id { get; set; }
        public int dmId { get; set; }
        public string DiCosaSiOccupaLAzienda { get; set; }
        public string ObiettivoPrincipale { get; set; }
        public string Target { get; set; }
        public string ElementiPrincipali { get; set; }
        public string Logo { get; set; }
        public string sessionUser { get; set; }
    
        public virtual Dm Dm { get; set; }
    }
}
