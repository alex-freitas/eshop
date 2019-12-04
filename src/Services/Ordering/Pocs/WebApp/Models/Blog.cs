using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class Blog
    {
        public Blog()
        {
            Posts = new List<Post>();
        }

        public int BlogId { get; set; }
        
        public string Url { get; set; }
        
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; } 

        public ICollection<Post> Posts { get; set; }

        public Settings Settings { get; set; }
    }

    public class Settings
    {
        public bool HideOldPosts { get; set; } 
    }
}
