using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocAndDine.Models
{
    public class Blog
    {
        public int blogId { get; set; }
        public int bloggerId { get; set; }
        public string blogName { get; set; }
        public string blogPic { get; set; }
        public DateTime date { get; set; }
        public string blogSummary { get; set; }
        public string blogDescription { get; set; }
        public int userId { get; set; }
        public int blogCommentId { get; set; }
        public string blogComment { get; set; }

    }
}