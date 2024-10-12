using MusicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AuthController : Controller
    {
        public DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
        // GET: Auth
        public ActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        public ActionResult Register(FormCollection form)
        {
            string username = form["username"];
            string password = form["password"];
            string confirmpassword = form["ConfirmPassword"];
            var user = db.mic_users.FirstOrDefault(x => x.login_name == username);
            if(user != null)
            {
                ViewBag.Message = "Username is exist!";
                return View();
            }
            else
            {
                if(password != confirmpassword)
                {
                    ViewBag.Message = "Confirm password does not match the password!";
                    return View();
                }
                else
                {
                    mic_user newuser = new mic_user();
                    newuser.login_name = username;
                    newuser.password = password;
                    db.mic_users.InsertOnSubmit(newuser);
                    db.SubmitChanges();
                    ViewBag.Message = "Sign up successful!";
                }
            }
            return RedirectToAction("Login", "Auth");
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.mic_users.FirstOrDefault(x => x.login_name == model.login_name && x.password == model.password);
                if (user != null)
                {
                    Session["user_id"] = user.user_id;
                    Session["login_name"] = user.login_name;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }
            return View(model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

    }
}
