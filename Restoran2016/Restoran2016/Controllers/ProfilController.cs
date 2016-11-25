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
        private aco4Entities db = new aco4Entities();
     
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
                             select new ModelView.RezervisaniRestoran { ID = y.ID, ID_RESTORANA = y.ID_RESTORANA, ID_STOLA = y.ID_STOLA, DATUM = y.DATUM, OCENA = y.OCENA, VREME_DOLASKA = y.VREME_DOLASKA, VREME_ODLASKA = y.VREME_ODLASKA,NAZIV_RESTORANA = z.NAZIV_RESTORANA };

                IEnumerable<Restoran2016.ModelView.RezervisaniRestoran> poseteLista = posete;

                var pozivi = from y in db.PRIJATELJI_REZERVACIJA
                             join z in db.REZERVACIJAs on y.ID equals z.ID
                             join x in db.RESTORANs on z.ID_RESTORANA equals x.ID_RESTORANA
                             join w in db.GOSTs on y.EMAIL_GOSTA1 equals w.EMAIL_GOSTA
                             
                             where y.EMAIL_GOSTA == id
                             select new ModelView.PozivRestoran {EMAIL_GOSTA=y.EMAIL_GOSTA,EMAIL_GOSTA1=y.EMAIL_GOSTA1,ID=z.ID,OCENA=y.OCENA,DATUM=z.DATUM,VREME_DOLASKA=z.VREME_ODLASKA,VREME_ODLASKA=z.VREME_ODLASKA,ID_STOLA=z.ID_STOLA,NAZIV_RESTORANA=x.NAZIV_RESTORANA, IME=w.IME_GOSTA,PREZIME=w.PREZIME_GOSTA };

                IEnumerable<Restoran2016.ModelView.PozivRestoran> poziviLista = pozivi;        
                
                var tuple = new Tuple<ModelView.Profil, IEnumerable<Restoran2016.ModelView.RezervisaniRestoran>,IEnumerable<Restoran2016.ModelView.PozivRestoran>>(prof, poseteLista,poziviLista);

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
            if (rest.LATITUDA == null || rest.LONGITUDA == null)
                ViewBag.MapUrl = MapAddress(rest.ADRESA_RESTORANA, 10000, "ROAD", 240, 320);
            else
            {
                ViewBag.MapUrl = MapCoordinates((double)rest.LATITUDA, (double)rest.LONGITUDA, 10000, "ROAD", 240, 320);
            }
            
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
        public ActionResult OceniRestoranPoz(int id,string email1, string email)
        {
            PRIJATELJI_REZERVACIJA pr = db.PRIJATELJI_REZERVACIJA.Find(email1, email,id);
            
            return View(pr);
        }

        [HttpPost]
        public ActionResult OceniRestoranPoz(PRIJATELJI_REZERVACIJA pr)
        {

            if (ModelState.IsValid)
            {

                db.Entry(pr).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profil");
            }

            return View(pr);
        }

        [HttpGet]
        public ActionResult RezervisiRestoran(string id)
        {

            string email = Session["idGosta"].ToString();
            var genID = db.REZERVACIJAs.Count() + 1;

            RESTORAN rest = db.RESTORANs.Find(id);
            REZERVACIJA rez = new REZERVACIJA();
            rez.EMAIL_GOSTA = email;
            rez.ID = genID;
            rez.ID_RESTORANA = rest.ID_RESTORANA;
            if (rest.BROJ_STOLOVA > 0)
                return View(rez);
            else
                ViewBag.Message = "Nema dostupnih stolova u ovom restoranu";
                return View(rez);
        }
        [HttpPost]
        public ActionResult RezervisiRestoran(REZERVACIJA rez)
        {
            DateTime datum = rez.DATUM;
            DateTime dolazak = new DateTime(rez.DATUM.Year, rez.DATUM.Month, rez.DATUM.Day, rez.VREME_DOLASKA.Hour, rez.VREME_DOLASKA.Minute, rez.VREME_DOLASKA.Second);
            DateTime odlazak = new DateTime(rez.DATUM.Year, rez.DATUM.Month, rez.DATUM.Day, rez.VREME_ODLASKA.Hour, rez.VREME_ODLASKA.Minute, rez.VREME_ODLASKA.Second);
            rez.VREME_DOLASKA = dolazak;
            rez.VREME_ODLASKA = odlazak;
           // var slobodniStolovi = from x in db.REZERVACIJAs where !(x.DATUM == rez.DATUM && ((rez.VREME_DOLASKA <= x.VREME_DOLASKA && rez.VREME_ODLASKA >= x.VREME_DOLASKA) || (rez.VREME_DOLASKA >= x.VREME_DOLASKA && rez.VREME_ODLASKA <= x.VREME_ODLASKA) || (rez.VREME_DOLASKA <= x.VREME_ODLASKA && rez.VREME_ODLASKA >= x.VREME_ODLASKA))) select x.ID_STOLA;
            var sviRezervisani = from x in db.REZERVACIJAs where (x.DATUM == rez.DATUM && x.ID_RESTORANA==rez.ID_RESTORANA && ((rez.VREME_DOLASKA <= x.VREME_DOLASKA && rez.VREME_ODLASKA >= x.VREME_DOLASKA) || (rez.VREME_DOLASKA >= x.VREME_DOLASKA && rez.VREME_ODLASKA <= x.VREME_ODLASKA) || (rez.VREME_DOLASKA <= x.VREME_ODLASKA && rez.VREME_ODLASKA >= x.VREME_ODLASKA))) select x.ID_STOLA;
            var sviStolovi = from x in db.STOes where x.ID_RESTORANA == rez.ID_RESTORANA select x.ID_STOLA;
            var sviSlobodni = sviStolovi.Except(sviRezervisani);

            List<String> lista = new List<String>();


            foreach (string num in sviSlobodni)
            {
                lista.Add(num);
            }

            TempData["checkboxovi"] = lista;
            TempData["datum"] = datum;
            TempData["dolazak"] = dolazak;
            TempData["odlazak"] = odlazak;
            if (dolazak < odlazak)
            {
                return RedirectToAction("RezervisiRestoran2", new { id = rez.ID_RESTORANA });
            }
            else ModelState.AddModelError("", "Pocetno vreme ne sme biti posle krajnjeg vremena!");
             return View(rez);

        }
        [HttpGet]
        public ActionResult RezervisiRestoran2(string id)
        {

            RESTORAN r = db.RESTORANs.Find(id);
            ViewBag.BrojRedova = r.BROJ_KOLONA; //prekoprofilamenadzera
            int stolova = (from x in db.STOes where x.ID_RESTORANA == id select x).Count();
            ViewBag.BrojStolova = stolova;
            //var cb = TempData["ckeckboxovi"];
            TempData["r"] = r.ID_RESTORANA;
            List<string> lista = TempData["checkboxovi"] as List<string>;
            ViewBag.listaDostupnih = lista;
            return View();


        }
    
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

            using (var db = new aco4Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.napuni_rezervacije(99, rez.EMAIL_GOSTA, rez.ID_RESTORANA, id, rez.DATUM, rez.VREME_DOLASKA, rez.VREME_ODLASKA);
                        db.SaveChanges();

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            } 

           // db.napuni_rezervacije(99, rez.EMAIL_GOSTA, rez.ID_RESTORANA, id, rez.DATUM, rez.VREME_DOLASKA, rez.VREME_ODLASKA);
            //db.SaveChanges();
            
            List<string> lista = TempData["checkboxovi"] as List<String>;
            //       lista.Remove(id);
            TempData["rez"] = rez;

            return RedirectToAction("PozoviPrijatelje");

        }
        [HttpGet]
        public ActionResult PozoviPrijatelje()
        {
            REZERVACIJA rez = TempData["rez"] as REZERVACIJA;
            TempData["rez1"] = TempData["rez"];
            var prijatelji = (db.PRIJATELJIs.Where(y => y.EMAIL_GOSTA1 == rez.EMAIL_GOSTA).Select(y => y.EMAIL_GOSTA));
            List<PRIJATELJI> listaPri = new List<PRIJATELJI>();
            int brojPri = 0;
            foreach (var item in prijatelji)
            {
                listaPri.Add(db.PRIJATELJIs.Where(x => x.EMAIL_GOSTA == item).Single());
                brojPri++;
            }
            var model = new PoziviPrijatelja
            {
                SelektovaniPr = new[] { "1" },
                prijateljiGosta = listaPri.Select(x => new SelectListItem
                {
                    Value = x.EMAIL_GOSTA,
                    Text = x.GOST.IME_GOSTA+" "+x.GOST.PREZIME_GOSTA
                })
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult PozoviPrijatelje(PoziviPrijatelja lista)
        {

            if (lista.SelektovaniPr==null)
                return RedirectToAction("Profil");
            for (int i = 0; i < lista.SelektovaniPr.Count(); i++)
            {
                REZERVACIJA rez = TempData["rez1"] as REZERVACIJA;
                var rezID = (from y in db.REZERVACIJAs where y.EMAIL_GOSTA == rez.EMAIL_GOSTA && y.DATUM == rez.DATUM && y.ID_RESTORANA == rez.ID_RESTORANA && y.VREME_DOLASKA == rez.VREME_DOLASKA select y.ID).FirstOrDefault(); // moze umjesto svega ovog y.ID==rez.ID, ali ovo moze biti pogodno za visestruke rezervacije
                String rezIDD = rezID.ToString();
                PRIJATELJI_REZERVACIJA pr = new PRIJATELJI_REZERVACIJA();
                pr.EMAIL_GOSTA1 = Session["idgosta"].ToString();
                pr.EMAIL_GOSTA = lista.SelektovaniPr[i];
                pr.ID = Int32.Parse(rezIDD);
                pr.OCENA = null;
                db.PRIJATELJI_REZERVACIJA.Add(pr);
                db.SaveChanges();

            }
            return Redirect(Url.RouteUrl(new { controller = "Profil", action = "Profil" }) + "#tabs-3");
        }

     
        public ActionResult PrihvatiPoziv(int id, String email1, String email)
        {
            PRIJATELJI_REZERVACIJA rez = db.PRIJATELJI_REZERVACIJA.Find(email1, email, id);
            rez.OCENA = 0;
            db.Entry(rez).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(Url.RouteUrl(new { controller = "Profil", action = "Profil" }) + "#tabs-3");
        }

        public ActionResult OdbijPoziv(int id, String email1, String email)
        {
            PRIJATELJI_REZERVACIJA rez = db.PRIJATELJI_REZERVACIJA.Find(email1, email, id);
            db.PRIJATELJI_REZERVACIJA.Remove(rez);
            db.SaveChanges();
            return Redirect(Url.RouteUrl(new { controller = "Profil", action = "Profil" }) + "#tabs-3");
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
         if (latlong == null) { double latitude = 0; double longitude = 0; return GetMapUri(latitude, longitude, zoom, mapStyle, width, height); }
         else
         {
             double latitude = latlong.Latitude;
             double longitude = latlong.Longitude;
             return GetMapUri(latitude, longitude, zoom, mapStyle, width, height);
         }

     }
     private string MapCoordinates(double latitude, double longitude, int zoom, string mapStyle, int width, int height)
     {
         return GetMapUri(latitude, longitude, zoom, mapStyle, width, height);
     }
	}
}