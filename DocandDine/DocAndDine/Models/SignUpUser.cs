using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocAndDine.Models
{
    public class SignUpUser
    {
        [Required]
        [Display(Name ="User Name")]
        [StringLength(20,ErrorMessage = "Please Enter valid user name. Name must not exceed 20 character")]
        public string userName { get; set; }

        [Required]
        [RegularExpression(".+\\@.+\\..+",ErrorMessage ="Please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password you have entered doesnt match. Or incorrect Password.")]
        public string password { get; set; }

        [Required(ErrorMessage = "The password you have entered doesnt match. Or incorrect Password.")]
        [Compare("Password", ErrorMessage = "The password you have entered doesnt match. Or incorrect Password.")]
        public string rePassword{ get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "Please enter valid phone number.")]
        public string phoneNo { get; set; }

        public string path { get; set; }
    }
}