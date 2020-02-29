using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class ApplicationUser : IdentityUser
    {
        //public int userId { get; set; } replaced by Id
        // public string username { get; set; } replaced by UserName
        //public string email { get; set; } replaced by Email
        //public string userPassword { get; set; } replaced by PasswordHash
        public decimal Balance { get; set; }
        //public bool isEmailVerified { get; set; } replaced by EmailConfirmed
        //public System.Guid activationCode { get; set; } TBD
    }
}
