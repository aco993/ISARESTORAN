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
    
    public partial class JELOVNIK
    {
        public string ID_RESTORANA { get; set; }
        public string ID_JELA { get; set; }
        public string NAZIV_JELA { get; set; }
        public string OPIS { get; set; }
        public decimal CENA_JELA { get; set; }
    
        public virtual RESTORAN RESTORAN { get; set; }
    }
}
