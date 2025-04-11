using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Pages.Blogs
{
    public class MyPostsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MyPostsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Blog> BlogList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToPage("/Index");

            var user = await _userManager.GetUserAsync(User);

            BlogList = await _context.Blogs
                .Include(b => b.Category)
                .Where(b => b.UserId == user.Id)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();

            return Page();
        }
    }
}
