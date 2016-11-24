using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Restoran2016.Models;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Claims;
using System.Net.Security;

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
                        gost.MAIL_POTVRA = false;
                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
    new System.Net.Mail.MailAddress("kupojeprodajic@gmail.com", "Registracija na web"),
    new System.Net.Mail.MailAddress(gost.EMAIL_GOSTA));
                    m.Subject = "Povrda e-mail adrese";
                    m.Body = string.Format("Postovani {0},<BR/>Hvala vam na registraciji, kliknite na link ispod da potvrdite: <a href=\"{1}\" title=\"User Email Confirm\">{1}</a>", gost.EMAIL_GOSTA, Url.Action("ConfirmEmail", "Account", new { Token = gost.EMAIL_GOSTA }, Request.Url.Scheme));
                    m.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                    smtp.Credentials = new System.Net.NetworkCredential("kupojeprodajic@gmail.com", "al255593");
                    smtp.EnableSsl = true;

                    ServicePointManager.ServerCertificateValidationCallback =
delegate(object s, X509Certificate certificate,
X509Chain chain, SslPolicyErrors sslPolicyErrors)
{ return true; };

                    smtp.Send(m);

                    db.GOSTs.Add(gost);
                    db.SaveChanges();


                    return RedirectToAction("CekajKonfirmaciju" );
                }
                else 
                                   
                {
                    ViewBag.Message = "Postoji korisnik sa ovom adresom!";
                    return View(gost);
                }
            }

            return View(gost);
        }

        public ActionResult CekajKonfirmaciju()
        {
            return View();
        }
        public ActionResult ConfirmEmail(string Token)
        {
                        GOST gost = db.GOSTs.Find(Token);
     
                         
                            gost.MAIL_POTVRA = true;
                            db.GOSTs.Attach(gost);
                            var entry = db.Entry(gost);
                            entry.Property(e => e.MAIL_POTVRA).IsModified = true;
                            db.SaveChanges();
                        
            return View();
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
                GOST gost2 = db.GOSTs.Find(gost.EMAIL_GOSTA);
                if (gost2.MAIL_POTVRA == true)
                {
                    Session["idGosta"] = gost.EMAIL_GOSTA;

                    return RedirectToAction("Profil", "Profil");
                }
                else {
                    ViewBag.Message = "Morate potvrditi vašu e-mail adresu da biste se prijavili!";
                    return View();
                }
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