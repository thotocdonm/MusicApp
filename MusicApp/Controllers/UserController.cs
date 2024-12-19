using MusicApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class UserController : Controller
    {
        private DataClasses1DataContext db = new DataClasses1DataContext("Data Source=DESKTOP-H3FAVBH;Initial Catalog=music;Integrated Security=True;TrustServerCertificate=True");

        // GET: User
        public ActionResult Index()
        {
            int userid = int.Parse(Session["user_id"].ToString());
            var x = db.mic_users.FirstOrDefault(u => u.user_id == userid);
            return View(x);
        }
        [HttpPost]
        public ActionResult EditUser(EditUserModel user)
        {
            var existingUser = db.mic_users.FirstOrDefault(u => u.user_id == user.id);
            if (ModelState.IsValid)
            {
                
                if (existingUser != null)
                {
                    existingUser.email = user.email;
                    existingUser.full_name = user.full_name;
                    existingUser.mobile_number = user.mobile_number;
                    existingUser.role = user.role;

                    db.SubmitChanges();
                }
                return RedirectToAction("Index", existingUser);
            }
            return View("Index", existingUser);
        }
        [HttpPost]
        public ActionResult EditPassword(EditUserModel user)
        {
            var existingUser = db.mic_users.FirstOrDefault(u => u.user_id == user.id);
            if (ModelState.IsValid)
            {
                
                if (existingUser != null)
                {
                    if (BCrypt.Net.BCrypt.Verify( user.password, existingUser.password))
                    {
                        if (user.newPassWord == user.cfPassWord) {
                            int cost = 10;
                            string hashPassword = BCrypt.Net.BCrypt.HashPassword(user.newPassWord, cost);
                            existingUser.password = hashPassword;
                            db.SubmitChanges();
                            TempData["successMessage"] = "Mật khẩu đã được thay đổi thành công!";
                            return RedirectToAction("Index", existingUser);
                        }
                        else { TempData["wrongPassMatch"] = "Khong trung khop"; }
                    }
                    else { TempData["wrongcurrPass"] = "Sai mat khau ban dau"; }

                }
            }
            return View("Index", existingUser);
        }
    }
}