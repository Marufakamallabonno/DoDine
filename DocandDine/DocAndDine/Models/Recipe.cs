using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocAndDine.Models
{
    public class Recipe
    {
        public int recipeId { get; set; }
        public int recipeWriterId { get; set; }
        public string recipeName { get; set; }
        public DateTime recipePublishDate { get; set; }
        public string recipePic { get; set; }
        public string cookingTime { get; set; }
        public string recipeServing { get; set; }
        public string recipeIngredients { get; set; }
        public string recipePicture { get; set; }
        public string recipeProcess { get; set; }
        public string recipeDescription { get; set; }
    }
}