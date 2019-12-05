using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public class Blog
    {
        public Blog()
        {
            Posts = new List<Post>();

            BlogStatusId = BlogStatus.On.Id;
        }

        public int BlogId { get; set; }

        public string Url { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Post> Posts { get; set; }

        public Settings Settings { get; set; }

        public int? OwnerId { get; set; } 

        public int BlogStatusId { get; set; }

        public BlogStatus BlogStatus { get; private set; }  
    }

    public class Settings
    {
        public bool HideOldPosts { get; set; }
    }
}
