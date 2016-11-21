using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Restoran2016.Models;
using System.Data;
using System.Data.Entity;

namespace Restoran2016.Controllers
{
    public class AccountController : Controller
    {
        private aco4Entities db = new aco4Entities();
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Gost/Create

        [HttpPost]
        public ActionResult Register(GOST gost)
        {

            var userWithSameEmail = db.GOSTs.Where(m => m.EMAIL_GOSTA == gost.EMAIL_GOSTA).SingleOrDefault();
            if (ModelState.IsValid)
            {
                if (userWithSameEmail == null)
                {
                    if (gost.PASS_GOSTA != gost.CPASS_GOSTA)
                    {
                        return View(gost);
                    }
                    else
                    db.GOSTs.Add(gost);
                    db.SaveChanges();

                        
                    return RedirectToAction("Login");
                }
                else 
                                   
                {
                    ViewBag.Message = "Postoji korisnik sa ovom adresom!";
                    return View(gost);
                }
            }

            return View(gost);
        }

        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Gost/Create

        [HttpPost]
        public ActionResult Login(GOST gost)
        {


 
            if (db.GOSTs.Where(x => x.EMAIL_GOSTA == gost.EMAIL_GOSTA).Select(z => z.PASS_GOSTA).SingleOrDefault() == gost.PASS_GOSTA)
            {
                Session["idGosta"] = gost.EMAIL_GOSTA;
              

                return RedirectToAction("Profil", "Profil");
            }
            else if (db.ADMINs.Where(x => x.ID_ADMIN == gost.EMAIL_GOSTA).Select(z => z.PASS_ADMINA).SingleOrDefault() == gost.PASS_GOSTA)
            {
                Session["idAdmina"] = gost.EMAIL_GOSTA;

                return RedirectToAction("ProfilAdmina", "ProfilAdmina");
            }


            else if (db.MENADZERs.Where(x => x.IDMENADZERA == gost.EMAIL_GOSTA).Select(z => z.IDMENADZERA).SingleOrDefault() == gost.EMAIL_GOSTA)
            {
                var men = db.MENADZERs.Where(x => x.IDMENADZERA == gost.EMAIL_GOSTA).Single();

                Session["idMenadzera"] = men.IDMENADZERA;
                Session["idRestorana"] = men.ID_RESTORANA;
                return RedirectToAction("ProfilMenadzera", "ProfilMenadzera");
            }

            else
            {
                ModelState.AddModelError("", "Korisnik sa unetim podacima ne postoji.");
                return View(gost);
            }

           // return RedirectToAction("Login");
        }

	}



}