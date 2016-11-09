using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Restoran2016.Models;

namespace Restoran2016.ModelView
{
    public class ProfilMenadzera
    {
        public String IDRestorana { get; set; }
        public String NazivRestorana { get; set; }
        public String OpisRestorana { get; set; }
        public List<JELOVNIK> jelovnici { get; set; }

        public int brojRedova = 5;
    }
}