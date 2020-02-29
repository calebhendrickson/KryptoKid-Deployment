using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class AccountPortfolioModel
    {
        // INITIALIZE VIEWBAGPROPERTY ARRAYS
        public List<string> Name { get; set; }
        public List<decimal> Value { get; set; }
        public int[] Shares { get; set; }
        public string Email { get; set; }
        public int Length { get; set; }
        public decimal Assets { get; set;  }
        public int Balance { get; set; }
           
    }
}
