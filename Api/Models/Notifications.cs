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
    
    public partial class Notifications
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool enabled { get; set; }
        public string usersId { get; set; }
        public int notificationType { get; set; }
    }
}
