//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BookPool.DataObjects.EDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderHeader
    {
        public int ID { get; set; }
        public string ClientUserID { get; set; }
        public System.DateTime OrderedOn { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
}