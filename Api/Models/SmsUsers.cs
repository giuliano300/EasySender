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
    
    public partial class SmsUsers
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string errcode { get; set; }
        public string extrainfo { get; set; }
        public string targetcode { get; set; }
        public int dlr_ok { get; set; }
        public int dlr_ko { get; set; }
        public int dlr_wait { get; set; }
        public string message { get; set; }
        public string sender { get; set; }
        public System.DateTime startdate { get; set; }
        public string upperlimit { get; set; }
        public System.DateTime date { get; set; }
        public bool paid { get; set; }
        public decimal price { get; set; }
        public Nullable<int> paymentMethod { get; set; }
        public bool planned { get; set; }
        public int quantity { get; set; }
        public decimal vat { get; set; }
        public decimal total { get; set; }
        public Nullable<int> userParentId { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
