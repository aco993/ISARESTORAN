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
    public class ProfilMenadzeraController : Controller
    {
        private aco4Entities db = new aco4Entities();
        //
        // GET: /ProfilMenadzera/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProfilMenadzera()
        {
            if (Session["idMenadzera"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string id = Session["idMenadzera"].ToString();

                Restoran2016.ModelView.ProfilMenadzera profil = new ModelView.ProfilMenadzera();
                var g = db.MENADZERs.Where(x => x.IDMENADZERA == id).Single();
                var r = db.RESTORANs.Where(x => x.ID_RESTORANA == g.ID_RESTORANA).Single();
                Restoran2016.ModelView.ProfilMenadzera prof = new ModelView.ProfilMenadzera();

                prof.NazivRestorana = g.RESTORAN.NAZIV_RESTORANA;
                prof.OpisRestorana = g.RESTORAN.OPIS_RESTPRANA;
                prof.IDRestorana = g.ID_RESTORANA;
                if (r.BROJ_KOLONA == null)
                    prof.brojRedova = 5;
                else
                prof.brojRedova = (int)r.BROJ_KOLONA;
               
                prof.ASD = g.ASD;
                prof.ID_Menadzera = g.IDMENADZERA;
                int broj2 = prof.brojRedova;
                ViewBag.BrojRedova = broj2;
                int broj = db.STOes.Where(x => x.ID_RESTORANA == g.ID_RESTORANA).Count();
                ViewBag.BrojStolova = broj;
                var j = db.JELOVNIKs.Where(z=>z.ID_RESTORANA==r.ID_RESTORANA).Select(z => z.ID_JELA).ToList();
                prof.jelovnici = new List<JELOVNIK>();
                foreach (var item in j)
                {
                    prof.jelovnici.Add(db.JELOVNIKs.Where(z => z.ID_JELA == item).Single());
                }
                if (r.LATITUDA == null || r.LONGITUDA == null)
                    ViewBag.MapUrl = MapAddress(r.ADRESA_RESTORANA, 10000, "ROAD", 240, 320);
                else {
                    ViewBag.MapUrl = MapCoordinates((double)r.LATITUDA, (double)r.LONGITUDA, 10000, "ROAD", 240, 320);
                }
                return View(prof);
            }

        }
        [HttpGet]
        public ActionResult Edit(string id)
        {

            MENADZER men = db.MENADZERs.Find(id);
            if (men == null)
            {
                return HttpNotFound();
            }

            return View(men);
        }

        [HttpPost]
        public ActionResult Edit(MENADZER men)
        {
    
            if (ModelState.IsValid)
            {
  
                db.Entry(men).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilMenadzera");
            }

            return View(men);
        }



        [HttpGet]
        public ActionResult EditRestoran()
        {
            var id = (string)Session["IDMenadzera"];

            var g = db.MENADZERs.Where(x => x.IDMENADZERA == id).SingleOrDefault();
            String idrest = db.MENADZERs.Where(z => z.IDMENADZERA == id).Select(z => z.ID_RESTORANA).SingleOrDefault().ToString();


            RESTORAN jel = db.RESTORANs.Find(idrest);
            if (jel == null)
            {
                return HttpNotFound();
            }

            return View(jel);
        }

        [HttpPost]
        public ActionResult EditRestoran(RESTORAN jel)
        {


            if (ModelState.IsValid)
            {

                db.Entry(jel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilMenadzera");
            }
            return View(jel);
        }
        [HttpGet]
        public ActionResult PromeniKoordinate(string id)
        {
            var idd = (string)Session["IDMenadzera"];

            var g = db.MENADZERs.Where(x => x.IDMENADZERA == idd).SingleOrDefault();
            String idrest = db.MENADZERs.Where(z => z.IDMENADZERA == idd).Select(z => z.ID_RESTORANA).SingleOrDefault().ToString();


            RESTORAN jel = db.RESTORANs.Find(idrest);
            if (jel == null)
            {
                return HttpNotFound();
            }

            return View(jel);
           
        }

        [HttpPost]
        public ActionResult PromeniKoordinate(RESTORAN rest)
        {


            if (ModelState.IsValid)
            {
                db.Entry(rest).State = EntityState.Modified;
                db.SaveChanges();
                
                return RedirectToAction("ProfilMenadzera");
            }
            return View(rest);
        }

        [HttpGet]
        public ActionResult UbaciStolove(string idRest)
        {
            TempData["idRest"] = idRest;
            RESTORAN rest = db.RESTORANs.Find(idRest);
            return View(rest);
        }

        [HttpPost]
        public ActionResult UbaciStolove(RESTORAN rest)
        {
          
            db.unosStolova(rest.BROJ_STOLOVA, rest.ID_RESTORANA);
            db.SaveChanges();
            return RedirectToAction("ProfilMenadzera");
        }

        [HttpGet]
        public ActionResult CreateJelovnik()
        {

            return View();
        }

        [HttpPost]
        public ActionResult CreateJelovnik(JELOVNIK jelo)
        {

            string sesija = (string)Session["IDMenadzera"];
            String idrest = db.MENADZERs.Where(z => z.IDMENADZERA == sesija).Select(z => z.ID_RESTORANA).Single().ToString();
            jelo.ID_RESTORANA = idrest;
            if (ModelState.IsValid)
            {

                db.JELOVNIKs.Add(jelo);

                db.SaveChanges();
                return RedirectToAction("ProfilMenadzera");
            }
            return View();

        }

        public ActionResult EditJelovnik(string re, string id)
        {
            String idrest = db.JELOVNIKs.Where(z => z.ID_JELA == id).Select(z => z.ID_RESTORANA).Single().ToString();


            JELOVNIK jel = db.JELOVNIKs.Find(idrest, id);
            if (jel == null)
            {
                return HttpNotFound();
            }
            string re2 = jel.ID_RESTORANA;

            return View(jel);
        }



        //
        // POST: /Gost/Edit/5

        [HttpPost]
        public ActionResult EditJelovnik(JELOVNIK jel)
        {
            string sesija = (string)Session["IDMenadzera"];
            String idrest = db.MENADZERs.Where(z => z.IDMENADZERA == sesija).Select(z => z.ID_RESTORANA).Single().ToString();
            jel.ID_RESTORANA = idrest;

            if (ModelState.IsValid)
            {
                db.Entry(jel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilMenadzera");
            }
            return View(jel);
        }

        [HttpGet]
        public ActionResult DeleteJelovnik(string id)
        {

            String idrest = db.JELOVNIKs.Where(z => z.ID_JELA == id).Select(z => z.ID_RESTORANA).Single().ToString();

            JELOVNIK jel = db.JELOVNIKs.Find(idrest, id);

            if (jel == null)
            {
                return HttpNotFound();
            }
            return View(jel);
        }

        [HttpPost, ActionName("DeleteJelovnik")]
        public ActionResult DeleteJelovnikConfirmed(string id)
        {
            string sesija = (string)Session["IDMenadzera"];
            String idrest = db.MENADZERs.Where(z => z.IDMENADZERA == sesija).Select(z => z.ID_RESTORANA).Single().ToString();
            JELOVNIK jel = db.JELOVNIKs.Find(idrest, id);
            db.JELOVNIKs.Remove(jel);
            db.SaveChanges();
            return RedirectToAction("ProfilMenadzera");
        }

       
        public ActionResult PodesiKonfiguraciju(string brRed) {
            //kolone i redove sam obrnuo u zapisu, logika je dobra
            string id = (string)Session["IDMenadzera"];
            Restoran2016.ModelView.ProfilMenadzera prof = new ModelView.ProfilMenadzera();
            var g = db.MENADZERs.Where(x => x.IDMENADZERA == id).Single();
            var r = db.RESTORANs.Where(x => x.ID_RESTORANA == g.ID_RESTORANA).Single();
            
           int broj2=0;
           if (brRed == null) { brRed = r.BROJ_KOLONA.ToString(); }
            broj2 = Int32.Parse(brRed);
            ViewBag.BrojRedova = broj2;
            int broj = db.STOes.Where(x => x.ID_RESTORANA == g.ID_RESTORANA).Count();
            ViewBag.BrojStolova = broj;
            
            return View();
        
        }

        [HttpPost]
        public ActionResult PromeniKonfiguraciju(RESTORAN rest)
        {

            string sesija = (string)Session["IDMenadzera"];
            String idrest = db.MENADZERs.Where(z => z.IDMENADZERA == sesija).Select(z => z.ID_RESTORANA).Single().ToString();

            if (ModelState.IsValid)
            {
                db.Entry(rest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilMenadzera");
            }
            return View(rest);
        }


        [HttpGet]
        public ActionResult PromeniKonfiguraciju(string id)
        {
            RESTORAN rest = db.RESTORANs.Find(id);
            if (rest == null)
            {
                return HttpNotFound();
            }

            return View(rest);       
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
            else { 
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