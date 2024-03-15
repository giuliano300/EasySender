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
    
    public partial class Dm
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Dm()
        {
            this.NamesDm = new HashSet<NamesDm>();
            this.DmConfigurations = new HashSet<DmConfigurations>();
        }
    
        public int id { get; set; }
        public int userId { get; set; }
        public int productId { get; set; }
        public bool haveCreativity { get; set; }
        public int recipientsType { get; set; }
        public System.DateTime date { get; set; }
        public int numberOfNames { get; set; }
        public decimal netPrice { get; set; }
        public decimal vatPrice { get; set; }
        public decimal totalPrice { get; set; }
        public string sessionUser { get; set; }
        public Nullable<int> paymentMethod { get; set; }
        public bool complete { get; set; }
        public bool paid { get; set; }
        public Nullable<int> userParentId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NamesDm> NamesDm { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DmConfigurations> DmConfigurations { get; set; }
        public virtual Users Users { get; set; }
    }
}
