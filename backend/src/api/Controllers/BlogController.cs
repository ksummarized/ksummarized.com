using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public BlogController(ApiDbContext context) 
        {
            _context = context;
        }

        [HttpGet]
        public object Get()
        {
            return _context.Blogs.Where(b => b.Title.Contains("Title"))
                .Select(c => new Blog
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description
                })
                .ToList();
        }

        [HttpGet("{title}")]
        public object GetByTitle(string title)
        {
            return _context.Blogs.Where(b => b.Title == title)
                .Select(c => new Blog
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description
                })
                .ToList();
        }
    }
}
