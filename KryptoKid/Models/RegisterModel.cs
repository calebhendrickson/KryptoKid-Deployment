using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class RegisterModel
    {
        [Display(Name = "Username")]
        [Required, MaxLength(50)]
        public string username { get; set; }

        [Display(Name = "Email")]
        [Required, MaxLength(255)]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "Email Required fool")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "isEmailInUse", controller: "Account")]
        public string email { get; set; }

        [Required, DataType(DataType.Password)]
        public string userPassword { get; set; }

        //[Display(Name = " Confirm Password")]
        [DataType(DataType.Password), Compare(nameof(userPassword), ErrorMessage = "Password and confirmation password do not match.")]
        public string confirmPassword { get; set; }
    }
}
