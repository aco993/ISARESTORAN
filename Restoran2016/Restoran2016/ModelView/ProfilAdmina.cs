using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Restoran2016.Models;

namespace Restoran2016.ModelView
{
    public class ProfilAdmina
    {
        //public string NAZIV_RESTORANA { get; set; }
        public string ID_ADMIN { get; set; }
        public List<RESTORAN> restorani { get; set; }
        public List<MENADZER> menadzeri { get; set; }



    }
}