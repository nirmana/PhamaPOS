//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PhamaPOS_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class unit
    {
        public unit()
        {
            this.items = new HashSet<item>();
        }
    
        public int unitId { get; set; }
        public string unitCode { get; set; }
        public string unitDescription { get; set; }
        public Nullable<bool> unitStatus { get; set; }
    
        public virtual ICollection<item> items { get; set; }
    }
}
