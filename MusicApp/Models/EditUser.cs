using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicApp.Models
{
    public class EditUserModel
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string login_name { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string mobile_number { get; set; }
        [Required]
        public string full_name { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string newPassWord { get; set; }
        [Required]
        public string cfPassWord { get; set; }
        [Required]
        public string role { get; set; }
    }

}