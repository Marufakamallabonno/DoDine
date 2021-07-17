using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DocAndDine.Models
{
    public class Contact
    {
        public int contactId { get; set; }
        
        [Required]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter valid email")]
        public string email { get; set; }

        [Required]
        [StringLength(11, ErrorMessage = "Please enter valid phone number.")]
        public string phoneno { get; set; }

        public string contactMessage { get; set; }
        public int userId { get; set; }

    }
}