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
    
    public partial class UserPermits
    {
        public int id { get; set; }
        public int userId { get; set; }
        public bool rol { get; set; }
        public bool lol { get; set; }
        public bool tol { get; set; }
        public bool mol { get; set; }
        public bool col { get; set; }
        public bool pec { get; set; }
        public bool pacchi { get; set; }
        public bool sms { get; set; }
        public bool marketing { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
