using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicApp.Models
{
    public class AddUser
    {
        [Required]
        public string new_login_name { get; set; }
        [Required]
        public string new_email { get; set; }
        [Required]
        public string new_mobile_number { get; set; }
        [Required]
        public string new_role { get; set; }
        [Required]
        public string new_full_name { get; set; }
        [Required]
        public string new_password { get; set; }
        /*        [Required]
                public string new_created_by { get; set; }
                [Required]
                public string new_modified_by { get; set; }*/
    }
}