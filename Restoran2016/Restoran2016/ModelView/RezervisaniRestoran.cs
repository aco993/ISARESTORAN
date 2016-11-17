using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restoran2016.ModelView
{
    public class RezervisaniRestoran
    {
        public int ID { get; set; }
        public string ID_RESTORANA { get; set; }
        public string NAZIV_RESTORANA { get; set; }
        public string ID_STOLA { get; set; }
        public int OCENA { get; set; }
        public System.DateTime DATUM { get; set; }

        public System.DateTime VREME_DOLASKA { get; set; }
        public System.DateTime VREME_ODLASKA { get; set; }

    }
}