using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Restoran2016.Models
{
    public class PoziviPrijatelja
    {
        public IEnumerable<SelectListItem> prijateljiGosta { get; set; }
        public string[] SelektovaniPr { get; set; }
    }
}