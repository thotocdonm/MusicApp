﻿using MusicApp.Models;
using System.Linq;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AuthController : Controller
    {

        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=LAPTOP-DK8CPN92\\SQLEXPRESS;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");
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
            if (user != null)
            {
                TempData["ErrorMessage"] = "Username already exists!";
                return View();
            }
            else
            {
                if (password != confirmpassword)
                {
                    TempData["ErrorMessage"] = "Confirm password does not match the password!";
                    return View();
                }
                else
                {
                    mic_user newuser = new mic_user();
                    int cost = 10;
                    string hashPassword = BCrypt.Net.BCrypt.HashPassword(password, cost);
                    newuser.login_name = username;
                    newuser.password = hashPassword;
                    newuser.role = "user";
                    TempData["SuccessMessage"] = "Sign up successful!";
                    db.mic_users.InsertOnSubmit(newuser);
                    db.SubmitChanges();
                    return View("Register");
                }
            }
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
                var user = db.mic_users.FirstOrDefault(x => x.login_name == model.login_name);
                if (user != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(model.password, user.password))
                    {
                        Session["user_id"] = user.user_id;
                        Session["login_name"] = user.login_name;
                        Session["role"] = user.role;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["checkPassword"] = "Wrong password!";
                    }
                }
                else
                {
                    TempData["checkUser"] = "Invalid input. Please check your username and password.";
                }
            }
            else
            {
                ModelState.AddModelError("", "Please input your information.");
            }



            return View();
        }



        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

    }
}
