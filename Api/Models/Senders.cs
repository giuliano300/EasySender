//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Api.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Senders
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string businessName { get; set; }
        public string dug { get; set; }
        public string address { get; set; }
        public string houseNumber { get; set; }
        public string cap { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public string state { get; set; }
        public int operationId { get; set; }
        public string complementNames { get; set; }
        public string complementAddress { get; set; }
        public Nullable<bool> AR { get; set; }
        public string email { get; set; }
        public string telefono { get; set; }
    
        public virtual Operations Operations { get; set; }
    }
}
