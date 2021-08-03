//------------------------------------------------------------------------------
// <auto-generated>
//     Der Code wurde von einer Vorlage generiert.
//
//     Manuelle Änderungen an dieser Datei führen möglicherweise zu unerwartetem Verhalten der Anwendung.
//     Manuelle Änderungen an dieser Datei werden überschrieben, wenn der Code neu generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class TaxDeclaration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TaxDeclaration()
        {
            this.TaxDeclarationEntries = new HashSet<TaxDeclarationEntry>();
        }
    
        public int id { get; set; }
        public int year { get; set; }
        public bool isApproved { get; set; }
        public bool isSent { get; set; }
        public System.DateTime createdAt { get; set; }
        public System.DateTime modifiedAt { get; set; }
        public int person_id { get; set; }
    
        public virtual Person Person { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TaxDeclarationEntry> TaxDeclarationEntries { get; set; }
    }
}
