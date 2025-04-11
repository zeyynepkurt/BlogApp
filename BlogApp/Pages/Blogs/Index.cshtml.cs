using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Pages.Blogs
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Blog> Blog { get; set; } = new List<Blog>();

        [BindProperty(SupportsGet = true)]
        public int? SelectedCategoryId { get; set; }

        public SelectList Categories { get; set; }

        public async Task OnGetAsync()
        {
            // Kategorileri dropdown için al
            Categories = new SelectList(_context.Categories, "Id", "Name");

            // Blogları kategoriye göre filtrele
            var blogsQuery = _context.Blogs
                .Include(b => b.Category)
                .AsQueryable();

            if (SelectedCategoryId.HasValue)
            {
                blogsQuery = blogsQuery.Where(b => b.CategoryId == SelectedCategoryId.Value);
            }

            Blog = await blogsQuery.ToListAsync();
        }
    }
}
