using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DocAndDine.Models
{
    public class Login
    {
        [Required]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter valid email")]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}