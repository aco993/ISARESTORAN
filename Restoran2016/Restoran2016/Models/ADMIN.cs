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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    public partial class ADMIN
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [DisplayName("Email")]
        public string ID_ADMIN { get; set; }
        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DisplayName("Lozinka")]
        [DataType(DataType.Password)]
        public string PASS_ADMINA { get; set; }
    }
}
