﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Models;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ILogger<BlogController> _logger;
        private readonly BloggingContext _context;

        public BlogController(ILogger<BlogController> logger, BloggingContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public IEnumerable<Blog> Get()
        {
            string dbName = "blog.db";

            if (System.IO.File.Exists(dbName))
            {
                System.IO.File.Delete(dbName);
            }

            _context.Database.EnsureCreated();

            if (!_context.Blogs.Any())
            {
                var status = Enumeration.GetAll<BlogStatus>();
                _context.BlogStatus.AddRange(status);
                _context.SaveChanges();
                
                _context.Owner.Add(new Owner(1, "John Doe"));
                _context.SaveChanges();

                var newBlog = new Blog() { Title = Guid.NewGuid().ToString(), Url = "http://tempuri.org/", Settings = new Settings { HideOldPosts = true } };
                newBlog.Posts.Add(new Post { Title = Guid.NewGuid().ToString() });
                _context.Blogs.Add(newBlog);
                _context.SaveChanges();
            }

            var defaultBlog = _context.Blogs.FirstOrDefault();
            defaultBlog.Posts.Add(new Post { Title = Guid.NewGuid().ToString() });
            defaultBlog.OwnerId = 1;
            _context.Entry(defaultBlog).State = EntityState.Modified;
            _context.SaveChanges();

            var blogs = _context.Blogs.ToList();

            return blogs;            
        }
    }
}