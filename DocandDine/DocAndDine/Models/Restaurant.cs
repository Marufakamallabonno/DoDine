using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace DocAndDine.Models
{
    public class Restaurant
    {
        public int restaurantId { get; set; }
        [Required(ErrorMessage = "Name is Required.")]
        public string restaurantName { get; set; }
        [Required(ErrorMessage = "Location is Required.")]
        public string location { get; set; }
        [Required(ErrorMessage = "Area is Required.")]
        public string area { get; set; }
        public string offers { get; set; }
        public string profilePic { get; set; }
        public string coverPic { get; set; }
        public int menuId { get; set; }
        public string foodName { get; set; }
        public string foodIngredient { get; set; }
        public double Price { get; set; }
        public string resPassword { get; set; }



    }
}