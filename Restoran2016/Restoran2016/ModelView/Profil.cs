using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Restoran2016.Models;

namespace Restoran2016.ModelView
{
    public class Profil
    {
        public string Email { get; set; }
        public string ime { get; set; }
        public string prezime { get; set; }

        public String pass { get; set; }
        public String cpass { get; set; }

        public List<GOST> prijatelji { get; set; }
        public List<GOST> neprijatelji { get; set; }
        public List<GOST> zahtevi { get; set; }
        public List<RESTORAN> restorani { get; set; }
    }
}