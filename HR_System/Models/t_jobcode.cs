//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HR_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class t_jobcode
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public t_jobcode()
        {
            this.t_meridet = new HashSet<t_meridet>();
        }
    
        public string clave { get; set; }
        public string descrip { get; set; }
        public Nullable<decimal> mrp { get; set; }
        public string u_id { get; set; }
        public Nullable<System.DateTime> f_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<t_meridet> t_meridet { get; set; }
    }
}
