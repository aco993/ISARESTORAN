using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Restoran2016.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Restoran2016.ModelView
{
    public class Profil
    {
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Ime")]
        public string ime { get; set; }
        [DisplayName("Prezime")]
        public string prezime { get; set; }
        [DisplayName("Lozinka")]
        [DataType(DataType.Password)]
        public String pass { get; set; }
        [DisplayName("Potvrda lozinke")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("pass", ErrorMessage = "Lozinke se ne poklapaju.")]
        public String cpass { get; set; }

        public List<GOST> prijatelji { get; set; }
        public List<GOST> neprijatelji { get; set; }
        public List<GOST> zahtevi { get; set; }
        public List<RESTORAN> restorani { get; set; }


    }
}