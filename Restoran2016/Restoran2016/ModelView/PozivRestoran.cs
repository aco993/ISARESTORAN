using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Restoran2016.ModelView
{
    public class PozivRestoran
    {
        public string EMAIL_GOSTA1 { get; set; }
        public string EMAIL_GOSTA { get; set; }
        public string IME { get; set; }
        public string PREZIME { get; set; }
        public int ID { get; set; }
        public Nullable<int> OCENA { get; set; }
        public string ID_STOLA { get; set; }
        public System.DateTime VREME_DOLASKA { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH/mm/ss}", ApplyFormatInEditMode = true)]
        public System.DateTime VREME_ODLASKA { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public System.DateTime DATUM { get; set; }
        public string NAZIV_RESTORANA { get; set; }
    }
}