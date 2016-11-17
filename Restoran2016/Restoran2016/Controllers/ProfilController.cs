using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Restoran2016.Models;
using System.Data.Entity;
using Restoran2016.GeocodeServices;
using Restoran2016.ImageryServices; 

namespace Restoran2016.Controllers
{
    public class ProfilController : Controller
    {
        private acoEntities1 db = new acoEntities1();
     
        //
        // GET: /Profil/
        public ActionResult Profil()
        {                 

            if (Session["idGosta"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string id = Session["idGosta"].ToString();
                Restoran2016.ModelView.Profil profil = new ModelView.Profil();
                var g = db.GOSTs.Where(x => x.EMAIL_GOSTA == id).Single();

                Restoran2016.ModelView.Profil prof = new ModelView.Profil();
                prof.Email = id;
                prof.ime = g.IME_GOSTA;
                prof.prezime = g.PREZIME_GOSTA;
                prof.pass = g.PASS_GOSTA;
                prof.cpass = g.CPASS_GOSTA;
                
                var pr = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA1 == id).Select(z => z.EMAIL_GOSTA).ToList(); //lista poslatih zahteva
                var pr2 = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA == id).Select(z => z.EMAIL_GOSTA1).ToList(); //lista primljenih zahteva
                prof.prijatelji = new List<GOST>();
                var presjek = pr.Select(i => i.ToString()).Intersect(pr2); //prijatelji
                
                foreach (var item in presjek)
                {
                   prof.prijatelji.Add(db.GOSTs.Where(z => z.EMAIL_GOSTA == item).Single());

                }

                var zaht = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA == id).Where(z=>z.EMAIL_GOSTA1!=id).Select(z => z.EMAIL_GOSTA1).ToList();
                prof.zahtevi = new List<GOST>();

 
                    foreach (var item in zaht)
                    {  
                        prof.zahtevi.Add(db.GOSTs.Where(z => z.EMAIL_GOSTA == item).Single());
                    }

                    foreach (var item in pr)
                    {
                        prof.zahtevi.Remove(db.GOSTs.Where(z => z.EMAIL_GOSTA == item).Single());
                    }
                
                var osim = db.GOSTs.Where(z => z.EMAIL_GOSTA != id).Select(z => z.EMAIL_GOSTA).ToList();
                var npr = osim.Except(pr);
                prof.neprijatelji = new List<GOST>();
                foreach (var item in npr)
                {
                    prof.neprijatelji.Add(db.GOSTs.Where(z => z.EMAIL_GOSTA == item).Single());
                }

                var rest = db.RESTORANs.Select(z => z.ID_RESTORANA).ToList();
                prof.restorani = new List<RESTORAN>();

                foreach (var item in rest)
                {
                    prof.restorani.Add(db.RESTORANs.Where(z => z.ID_RESTORANA == item).Single());
                }
                ViewBag.restorani = prof.restorani;

                var posete = from y in db.REZERVACIJAs
                             join z in db.RESTORANs on y.ID_RESTORANA equals z.ID_RESTORANA 
                             where y.EMAIL_GOSTA == id 
                             select new ModelView.RezervisaniRestoran { ID=y.ID,ID_RESTORANA=y.ID_RESTORANA,ID_STOLA=y.ID_STOLA,DATUM=y.DATUM,OCENA=y.OCENA,NAZIV_RESTORANA=z.NAZIV_RESTORANA};

                IEnumerable<Restoran2016.ModelView.RezervisaniRestoran> poseteLista = posete;
                
                var tuple = new Tuple<ModelView.Profil, IEnumerable<Restoran2016.ModelView.RezervisaniRestoran>>(prof, poseteLista);

                return View(tuple);
            }

        }

        public ActionResult PronadjiPrijatelje(string ime, string prezime) {

            var gosti = from g in db.GOSTs
                         select g;

            if (!String.IsNullOrEmpty(ime)) {
                gosti = gosti.Where(x => x.IME_GOSTA.Contains(ime));
            }

            if (!String.IsNullOrEmpty(prezime))
            {
                gosti = gosti.Where(x => x.PREZIME_GOSTA.Contains(prezime));
            }

            return View(gosti);
        }

        public ActionResult PronadjiRestorane(string naziv, string vrsta, string sortOrder)
        {
            ViewBag.nazivSort=String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.vrstaSort=sortOrder=="Opis" ? "vrsta_desc" : "vrsta_asc";

            var restorani = from r in db.RESTORANs
                        select r;

            if (!String.IsNullOrEmpty(naziv))
            {
                restorani= restorani.Where(x => x.NAZIV_RESTORANA.Contains(naziv));
            }

            if (!String.IsNullOrEmpty(vrsta))
            {
                restorani = restorani.Where(x => x.OPIS_RESTPRANA.Contains(vrsta));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    restorani = restorani.OrderByDescending(s => s.NAZIV_RESTORANA);
                    break;
                case "vrsta_asc":
                    restorani = restorani.OrderBy(s => s.OPIS_RESTPRANA);
                    break;
                case "vrsta_desc":
                    restorani = restorani.OrderByDescending(s => s.OPIS_RESTPRANA);
                    break;
                default:
                    restorani = restorani.OrderBy(s => s.NAZIV_RESTORANA);
                    break;
            }

            return View(restorani);
        }

        public ActionResult InformacijeORestoranu(string id) {
            RESTORAN rest = db.RESTORANs.Find(id);
           var jel = from x in db.JELOVNIKs where (x.ID_RESTORANA == id) 
                                        select x.ID_JELA;

           List<JELOVNIK> jelLista = new List<JELOVNIK>();
           foreach (var item in jel)
           {
               jelLista.Add(db.JELOVNIKs.Where(x=>x.ID_JELA==item).Single());         
           }
           
            ViewBag.jel =jelLista;
            ViewBag.MapUrl = MapAddress(rest.ADRESA_RESTORANA, 10000, "ROAD", 240, 320);
        return View(rest);
        }

        [HttpGet]
        public ActionResult Dodaj(string id)
        {
            GOST gost = db.GOSTs.Find(id);

            if (gost == null)
            {
                return HttpNotFound();
            }
            return View(gost);
        }

        [HttpPost, ActionName("Dodaj")]
        public ActionResult DodajConfirmed(string id)
        {
            string posiljalac = Session["idGosta"].ToString();
            PRIJATELJI pr = new PRIJATELJI();
            pr.EMAIL_GOSTA1 = posiljalac;
            pr.EMAIL_GOSTA = id;
            pr.PRIJATELJI_OD = System.DateTime.Now;

            db.PRIJATELJIs.Add(pr);

            db.SaveChanges();
            return RedirectToAction("Profil");

        }
//TODO nepotrebne obe metode Dodaj i Dodaj2
        [HttpGet]
        public ActionResult Dodaj2(string id)
        {
            GOST gost = db.GOSTs.Find(id);

            if (gost == null)
            {
                return HttpNotFound();
            }
            string posiljalac = Session["idGosta"].ToString();


            return View(gost);
        }

        [HttpPost, ActionName("Dodaj2")]
        public ActionResult Dodaj2Confirmed(string id)
        {
            string posiljalac = Session["idGosta"].ToString();
            PRIJATELJI pr = new PRIJATELJI();
            pr.EMAIL_GOSTA1 = posiljalac;
            pr.EMAIL_GOSTA = id;
            pr.PRIJATELJI_OD = System.DateTime.Now;

            db.PRIJATELJIs.Add(pr);

            db.SaveChanges();
            return RedirectToAction("Profil");

        }
        [HttpGet]
        public ActionResult ObrisiP(string id)
        {
            GOST gost = db.GOSTs.Find(id);

            if (gost == null)
            {
                return HttpNotFound();
            }

            return View(gost);
        }

        [HttpPost, ActionName("ObrisiP")]
        public ActionResult ObrisiPConfirmed(string id)
        {
            string posiljalac = Session["idGosta"].ToString();
            PRIJATELJI pr = db.PRIJATELJIs.Find(posiljalac, id);

            db.PRIJATELJIs.Remove(pr);

            db.SaveChanges();
            return RedirectToAction("Profil");

        }

        [HttpGet]
        public ActionResult OdbijP(string id)
        {
            GOST gost = db.GOSTs.Find(id);

            if (gost == null)
            {
                return HttpNotFound();
            }

            return View(gost);
        }

        [HttpPost, ActionName("OdbijP")]
        public ActionResult OdbijPConfirmed(string id)
        {
            string posiljalac = Session["idGosta"].ToString();
            PRIJATELJI pr = db.PRIJATELJIs.Find(id, posiljalac);


            db.PRIJATELJIs.Remove(pr);

            db.SaveChanges();
            return RedirectToAction("Profil");

        }

        public ActionResult Edit(string id)
        {

            GOST gost = db.GOSTs.Find(id);
            if (gost == null)
            {
                return HttpNotFound();
            }
            ModelView.Profil prof = new ModelView.Profil();
            prof.ime = gost.IME_GOSTA;
            prof.prezime = gost.PREZIME_GOSTA;
            prof.Email = gost.EMAIL_GOSTA;
            prof.pass = gost.PASS_GOSTA;
            prof.cpass = gost.CPASS_GOSTA;

            return View(prof);
        }

        [HttpPost]
        public ActionResult Edit(ModelView.Profil prof)
        {
            GOST gost = db.GOSTs.Find(prof.Email);
            if (ModelState.IsValid)
            {
                gost.IME_GOSTA = prof.ime;
                gost.PREZIME_GOSTA = prof.prezime;
                gost.EMAIL_GOSTA = prof.Email;
                gost.PASS_GOSTA = prof.pass;
                gost.CPASS_GOSTA = prof.cpass;
                db.Entry(gost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profil");
            }

            return View(gost);
        }

        [HttpGet]
        public ActionResult OceniRestoran(int id) {

            REZERVACIJA rez = db.REZERVACIJAs.Find(id); 
            return View(rez);
        }

        [HttpPost]
        public ActionResult OceniRestoran(REZERVACIJA rez)
        {

            if (ModelState.IsValid)
            {

                db.Entry(rez).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profil");
            }

            return View(rez);
        }


        [HttpGet]
        public ActionResult RezervisiRestoran(string id) {
            
            string email=Session["idGosta"].ToString();
            var genID=db.REZERVACIJAs.Count()+1;

            RESTORAN rest = db.RESTORANs.Find(id);
            REZERVACIJA rez = new REZERVACIJA();
            rez.EMAIL_GOSTA = email;
            rez.ID = genID;
            rez.ID_RESTORANA = rest.ID_RESTORANA;
            return View(rez);

        
        }
        [HttpPost]
        public ActionResult RezervisiRestoran(REZERVACIJA rez)
        {
            var slobodniStolovi= from x in db.REZERVACIJAs where !(x.DATUM==rez.DATUM && ((rez.VREME_DOLASKA<x.VREME_DOLASKA && rez.VREME_ODLASKA>x.VREME_DOLASKA) || (rez.VREME_DOLASKA>x.VREME_DOLASKA && rez.VREME_ODLASKA<x.VREME_ODLASKA) || (rez.VREME_DOLASKA<x.VREME_ODLASKA && rez.VREME_ODLASKA>x.VREME_ODLASKA))) select x.ID_STOLA;
            var sviRezervisani = from x in db.REZERVACIJAs where (x.DATUM == rez.DATUM && ((rez.VREME_DOLASKA < x.VREME_DOLASKA && rez.VREME_ODLASKA > x.VREME_DOLASKA) || (rez.VREME_DOLASKA > x.VREME_DOLASKA && rez.VREME_ODLASKA < x.VREME_ODLASKA) || (rez.VREME_DOLASKA < x.VREME_ODLASKA && rez.VREME_ODLASKA > x.VREME_ODLASKA))) select x.ID_STOLA;
            var sviStolovi = from x in db.STOes where x.ID_RESTORANA == rez.ID_RESTORANA select x.ID_STOLA;
            var sviSlobodni = sviStolovi.Except(sviRezervisani);

            List<String> lista = new List<String>();
            DateTime datum = rez.DATUM;
            DateTime dolazak = rez.VREME_DOLASKA;
            DateTime odlazak = rez.VREME_ODLASKA;
         
            foreach (string num in sviSlobodni)
            {
                lista.Add(num);
            }
            
            TempData["checkboxovi"] = lista;
            TempData["datum"] = datum;
            TempData["dolazak"] = dolazak;
            TempData["odlazak"] = odlazak;
            return RedirectToAction("RezervisiRestoran2", new  {id=rez.ID_RESTORANA});


        }
        [HttpGet]
        public ActionResult RezervisiRestoran2(string id)
        {
            
            RESTORAN r = db.RESTORANs.Find(id);
            ViewBag.BrojRedova = r.BROJ_KOLONA; //prekoprofilamenadzera
            int stolova= (from x in db.STOes where x.ID_RESTORANA==id select x).Count();
            ViewBag.BrojStolova = stolova;
            //var cb = TempData["ckeckboxovi"];
            TempData["r"] = r.ID_RESTORANA;
            List<string> lista = TempData["checkboxovi"] as List<string>;
            ViewBag.listaDostupnih = lista; 
            return View();


        }
     [HttpPost]
        public ActionResult RezervisiRestoran3(string id)
        {

            String idgosta = Session["idGosta"].ToString();
           // var ubaciRezervaciju= from x in db.REZERVACIJAs
            REZERVACIJA rez = new REZERVACIJA();
            rez.EMAIL_GOSTA = idgosta;
            //rez.ID_STOLA = idStola;
            rez.DATUM = (System.DateTime)TempData["datum"];
            rez.VREME_DOLASKA = (System.DateTime)TempData["dolazak"];
            rez.VREME_ODLASKA = (System.DateTime)TempData["odlazak"];
            rez.ID_RESTORANA = (String)TempData["r"];
            db.napuni_rezervacije(99,rez.EMAIL_GOSTA, rez.ID_RESTORANA,id, rez.DATUM, rez.VREME_DOLASKA, rez.VREME_ODLASKA);
            db.SaveChanges();
            //db.REZERVACIJAs.Add()
            List<string> lista = TempData["checkboxovi"] as List<String>;
     //       lista.Remove(id);
           
            
            return RedirectToAction("Profil");
           


        }

     private GeocodeServices.Location GeocodeAddress(string address)
     {
         GeocodeRequest geocodeRequest = new GeocodeRequest();
         // Set the credentials using a valid Bing Maps Key
         geocodeRequest.Credentials = new GeocodeServices.Credentials();
         geocodeRequest.Credentials.ApplicationId = "7Xe5dWLJW6JIhH6jDYR7~6wrgqqAAstaYhxwKEyAB6g~AhB0MhPMBgrTWhG-cxBqbFxaJe92xk7oGVnJPfBckupM4wWTauJQIMkjs5Fs_Z5u";
         // Set the full address query
         geocodeRequest.Query = address;

         // Set the options to only return high confidence results 
         ConfidenceFilter[] filters = new ConfidenceFilter[1];
         filters[0] = new ConfidenceFilter();
         filters[0].MinimumConfidence = GeocodeServices.Confidence.High;
         GeocodeOptions geocodeOptions = new GeocodeOptions();
         geocodeOptions.Filters = filters;
         geocodeRequest.Options = geocodeOptions;
         // Make the geocode request
         GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
         GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

         if (geocodeResponse.Results.Length > 0)
             if (geocodeResponse.Results[0].Locations.Length > 0)
                 return geocodeResponse.Results[0].Locations[0];
         return null;
     }

     private string GetMapUri(double latitude, double longitude, int zoom, string mapStyle, int width, int height)
     {
         ImageryServices.Pushpin[] pins = new ImageryServices.Pushpin[1];
         ImageryServices.Pushpin pushpin = new ImageryServices.Pushpin();
         pushpin.Location = new ImageryServices.Location();
         pushpin.Location.Latitude = latitude;
         pushpin.Location.Longitude = longitude;
         pushpin.IconStyle = "2";
         pins[0] = pushpin;
         MapUriRequest mapUriRequest = new MapUriRequest();
         // Set credentials using a valid Bing Maps Key
         mapUriRequest.Credentials = new ImageryServices.Credentials();
         mapUriRequest.Credentials.ApplicationId = "7Xe5dWLJW6JIhH6jDYR7~6wrgqqAAstaYhxwKEyAB6g~AhB0MhPMBgrTWhG-cxBqbFxaJe92xk7oGVnJPfBckupM4wWTauJQIMkjs5Fs_Z5u";

         // Set the location of the requested image
         mapUriRequest.Pushpins = pins;
         // Set the map style and zoom level
         MapUriOptions mapUriOptions = new MapUriOptions();
         //mapUriOptions.ZoomLevel = 17;
         switch (mapStyle.ToUpper())
         {
             case "HYBRID":
                 mapUriOptions.Style = ImageryServices.MapStyle.AerialWithLabels;
                 break;
             case "ROAD":
                 mapUriOptions.Style = ImageryServices.MapStyle.Road;
                 break;
             case "AERIAL":
                 mapUriOptions.Style = ImageryServices.MapStyle.Aerial;
                 break;
             default:
                 mapUriOptions.Style = ImageryServices.MapStyle.Road;
                 break;
         }

         mapUriOptions.ZoomLevel = 15;
         // Set the size of the requested image to match the size of the image control
         mapUriOptions.ImageSize = new ImageryServices.SizeOfint();
         mapUriOptions.ImageSize.Height = height;
         mapUriOptions.ImageSize.Width = width;
         mapUriRequest.Options = mapUriOptions;

         ImageryServiceClient imageryService = new ImageryServiceClient("BasicHttpBinding_IImageryService");
         MapUriResponse mapUriResponse = imageryService.GetMapUri(mapUriRequest);

         return mapUriResponse.Uri;
     }

     private string MapAddress(string address, int zoom, string mapStyle, int width, int height)
     {
         GeocodeServices.Location latlong = GeocodeAddress(address);
          double latitude = latlong.Latitude;
         double longitude = latlong.Longitude;
         return GetMapUri(latitude, longitude, zoom, mapStyle, width, height);
     }

	}
}