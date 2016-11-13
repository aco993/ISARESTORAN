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
            

            //db.napuni_rezervacije(5, rez.EMAIL_GOSTA, rez.ID_RESTORANA);
            //db.SaveChanges();
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
            return RedirectToAction("Profil");
           


        }
	}
}