using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicApp.Models
{
    public class EditUserModel
    {
        public int id { get; set; }
        public string login_name { get; set; }
        public string email { get; set; }
        public string mobile_number { get; set; }
        public string full_name { get; set; }
        public string password { get; set; }
        public string newPassWord { get; set; }
        public string cfPassWord { get; set; }
        public string role { get; set; }
    }

}