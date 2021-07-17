using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocAndDine.Models
{
    public class HomeMadeFood
    {
        public int homeMadeFoodId { get; set; }
        public int homeChefId { get; set; }
        public string homemadeFoodName { get; set; }
        public string homemadeFoodPrice { get; set; }
        public string date { get; set; }
        public string homemadeFoodPic { get; set; }
        public string availabilityStatus{ get; set; }
      
    }
}