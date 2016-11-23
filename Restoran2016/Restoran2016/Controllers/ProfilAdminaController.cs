using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Restoran2016.Models;
using System.Data.Entity;

namespace Restoran2016.Controllers
{
    public class ProfilAdminaController : Controller
    {
        //
        // GET: /ProfilAdmina/
        private aco4Entities db = new aco4Entities();
       
        public ActionResult ProfilAdmina()
        {
            if (Session["idAdmina"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                string id = Session["idAdmina"].ToString();
                Restoran2016.ModelView.ProfilAdmina profil = new ModelView.ProfilAdmina();
                // var g = db.GOSTs.Where(x => x.EMAIL_GOSTA == id).Single();
                var a = db.ADMINs.Where(x => x.ID_ADMIN == id).Single();
                Restoran2016.ModelView.ProfilAdmina prof = new ModelView.ProfilAdmina();
                prof.ID_ADMIN = id;

                //var pr = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA1 == id).Select(z => z.EMAIL_GOSTA).ToList();

                var rest = db.RESTORANs.Select(z => z.ID_RESTORANA).ToList();
                prof.restorani = new List<RESTORAN>();
                foreach (var item in rest)
                {
                    prof.restorani.Add(db.RESTORANs.Where(z => z.ID_RESTORANA == item).Single());
                }

                var men = db.MENADZERs.Select(z => z.IDMENADZERA).ToList();
                prof.menadzeri = new List<MENADZER>();
                foreach (var item in men)
                {
                    prof.menadzeri.Add(db.MENADZERs.Where(z => z.IDMENADZERA == item).Single());
                }


                return View(prof);
            }

        }

        [HttpGet]
        public ActionResult CreateRestoran()
        {

            return View();
        }
        [HttpPost]
        public ActionResult CreateRestoran(RESTORAN restoran)
        {
            if (ModelState.IsValid)
            {
   
                restoran.BROJ_KOLONA = 1;
                db.RESTORANs.Add(restoran);

                
                for (int i = 1; i <= restoran.BROJ_STOLOVA; i++)
                {
                    STO sto = new STO();
                    sto.ID_STOLA = i.ToString();
                    sto.BR_STOLICA = 4;
                    sto.ID_RESTORANA = restoran.ID_RESTORANA;
                    db.STOes.Add(sto);
                    db.SaveChanges();
                    
                
                }
 
                    db.SaveChanges();
                return RedirectToAction("ProfilAdmina");
                
            }
            return View();

        }

        public ActionResult EditRestoran(string id)
        {
            RESTORAN gost = db.RESTORANs.Find(id);
            if (gost == null)
            {
                return HttpNotFound();
            }
            return View(gost);
        }



        //
        // POST: /Gost/Edit/5

        [HttpPost]
        public ActionResult EditRestoran(RESTORAN rest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilAdmina");
            }
            return View(rest);
        }

        [HttpGet]
        public ActionResult DeleteRestoran(string id)
        {
            //GOST gost = db.GOSTs.Find(id);
            RESTORAN rest = db.RESTORANs.Find(id);

            if (rest == null)
            {
                return HttpNotFound();
            }
            return View(rest);
        }

        [HttpPost, ActionName("DeleteRestoran")]
        public ActionResult DeleteRestoranConfirmed(string id)
        {
            RESTORAN rest = db.RESTORANs.Find(id);
            var brisanje = db.STOes.Where(x=>x.ID_RESTORANA==id);
            db.STOes.RemoveRange(brisanje);
            db.RESTORANs.Remove(rest);
            db.SaveChanges();
            return RedirectToAction("ProfilAdmina");
        }

        [HttpGet]
        public ActionResult CreateMenadzer()
        {
           var lista = db.RESTORANs.Select(x => x.NAZIV_RESTORANA );            
            ViewBag.restorani = lista;
            return View();
        }

        [HttpPost]
        public ActionResult CreateMenadzer(MENADZER menadzer)
        {
          
            var nazivRest = menadzer.RESTORAN.NAZIV_RESTORANA;
            var c = db.RESTORANs.Where(x => x.NAZIV_RESTORANA == nazivRest).Single();
            menadzer.ID_RESTORANA = c.ID_RESTORANA;
            menadzer.RESTORAN = c;

            //Add(db.RESTORANs.Where(z => z.ID_RESTORANA == item).Single());
            if (ModelState.IsValid)
            {
                
                db.MENADZERs.Add(menadzer); 
                db.SaveChanges();
                return RedirectToAction("ProfilAdmina");
            }
            return View();

        }

        public ActionResult EditMenadzer(string id)
        {
            MENADZER men = db.MENADZERs.Find(id);
            if (men == null)
            {
                return HttpNotFound();
            }
            return View(men);
        }

        //
        // POST: /Gost/Edit/5

        [HttpPost]
        public ActionResult EditMenadzer(MENADZER men)
        {
            if (ModelState.IsValid)
            {
                db.Entry(men).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProfilAdmina");
            }
            return View(men);
        }

        public ActionResult DeleteMenadzer(string id)
        {
            //GOST gost = db.GOSTs.Find(id);
            MENADZER men = db.MENADZERs.Find(id);

            if (men == null)
            {
                return HttpNotFound();
            }
            return View(men);
        }

        [HttpPost, ActionName("DeleteMenadzer")]
        public ActionResult DeleteMenadzerConfirmed(string id)
        {
            MENADZER rest = db.MENADZERs.Find(id);
            // GOST gost = db.GOSTs.Find(id);
            db.MENADZERs.Remove(rest);
            // db.GOSTs.Remove(gost);
            db.SaveChanges();
            return RedirectToAction("ProfilAdmina");
        }
	}
}