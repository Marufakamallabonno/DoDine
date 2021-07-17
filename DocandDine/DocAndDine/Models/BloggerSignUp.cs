using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocAndDine.Models
{
    public class BloggerSignUp
    {
        public string location { get; set; }

        public int age { get; set; }
        public int yearsOfBlogging { get; set; }
        public string education { get; set; }
        public string savoryOrSweet { get; set; }
        public string story { get; set; }
    }
}