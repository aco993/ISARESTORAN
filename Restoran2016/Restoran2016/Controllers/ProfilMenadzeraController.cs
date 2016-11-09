using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Restoran2016.Models;
using System.Data.Entity;

namespace Restoran2016.Controllers
{
    public class ProfilMenadzeraController : Controller
    {
        private acoEntities1 db = new acoEntities1();
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
                prof.brojRedova = (int)r.BROJ_KOLONA;
                prof.ASD = g.ASD;
                prof.ID_Menadzera = g.IDMENADZERA;
                int broj2 = prof.brojRedova;
                ViewBag.BrojRedova = broj2;
                int broj = db.STOes.Where(x => x.ID_RESTORANA == g.ID_RESTORANA).Count();
                ViewBag.BrojStolova = broj;
                var j = db.JELOVNIKs.Select(z => z.ID_JELA).ToList();
                prof.jelovnici = new List<JELOVNIK>();
                foreach (var item in j)
                {
                    prof.jelovnici.Add(db.JELOVNIKs.Where(z => z.ID_JELA == item).Single());
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


        
	}
}