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
    
    public partial class GED
    {
        public int id { get; set; }
        public string fileName { get; set; }
        public int userId { get; set; }
        public string pathFile { get; set; }
    
        public virtual Users Users { get; set; }
    }
}
