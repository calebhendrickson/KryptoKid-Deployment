using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "email required")]
        [EmailAddress]
        public string email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "password required")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Display(Name = "Remember Me")]
        public bool rememberMe { get; set; }
       
        //public string ReturnUrl { get; set; }

    }
}
