//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Restoran2016.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PRIJATELJI
    {
        public string EMAIL_GOSTA1 { get; set; }
        public string EMAIL_GOSTA { get; set; }
        public System.DateTime PRIJATELJI_OD { get; set; }
    
        public virtual GOST GOST { get; set; }
        public virtual GOST GOST1 { get; set; }
    }
}