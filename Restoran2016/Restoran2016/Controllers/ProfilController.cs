using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Restoran2016.Models;
using System.Data.Entity;

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
                

                var pr = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA1 == id).Select(z => z.EMAIL_GOSTA).ToList(); //lista poslatih zahteva
                var pr2 = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA == id).Select(z => z.EMAIL_GOSTA1).ToList(); //lista primljenih zahteva
                prof.prijatelji = new List<GOST>();
                var presjek = pr.Select(i => i.ToString()).Intersect(pr2); //prijatelji
                
                foreach (var item in presjek)
                {
                   prof.prijatelji.Add(db.GOSTs.Where(z => z.EMAIL_GOSTA == item).Single());

                }



              // var zaht = db.PRIJATELJIs.Where(z => z.EMAIL_GOSTA == id).Select(z => z.EMAIL_GOSTA1).ToList();
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

                return View(prof);
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

        public ActionResult PronadjiRestorane(string naziv, string vrsta)
        {

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

            return View(restorani);
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
            PRIJATELJI pr = db.PRIJATELJIs.Find(posiljalac,id);


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
                db.Entry(gost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profil");
            }

            return View(gost);
        }
	}
}