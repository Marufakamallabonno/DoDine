using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DocAndDine.Models
{
    public class Reservation
    {
        public int userId{ get; set; }
        public int restaurantId { get; set; }
        public string usename { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string restaurantname { get; set; }
    }
}