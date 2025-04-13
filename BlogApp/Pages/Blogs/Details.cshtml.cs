using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlogApp.Data;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;


namespace BlogApp.Pages.Blogs
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        

        public Blog Blog { get; set; }

        [BindProperty]
        public Comment NewComment { get; set; } = new Comment();

        public List<Comment> Comments { get; set; }


        private readonly UserManager<ApplicationUser> _userManager;


        public DetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public string CurrentUserId { get; set; } 

        public async Task<IActionResult> OnPostDeleteCommentAsync(int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            var currentUser = await _userManager.GetUserAsync(User);

            if (comment == null || comment.UserId != currentUser.Id)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id = comment.BlogId });
        }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            Blog = await _context.Blogs
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Blog == null)
                return NotFound();

            
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                CurrentUserId = user?.Id;
            }

            Comments = await _context.Comments
                .Where(c => c.BlogId == Blog.Id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Console.WriteLine("Yorum eklenmeye çalışılıyor");

            if (id == null)
                return NotFound();

            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError(string.Empty, "Yorum eklemek için giriş yapmalısınız.");
                await OnGetAsync(id);
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);

            NewComment.BlogId = id.Value;
            NewComment.CreatedAt = DateTime.Now;
            NewComment.Author = user.FullName;
            NewComment.UserId = user.Id;


            if (!ModelState.IsValid)
            {
                Console.WriteLine(" ModelState geçersiz!");
                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($" {state.Key}: {state.Value.Errors[0].ErrorMessage}");
                    }
                }

                await OnGetAsync(id); // sayfayı yenile
                return Page();
            }

            _context.Comments.Add(NewComment);
            await _context.SaveChangesAsync();

            Console.WriteLine(" Yorum kaydedildi");
            return RedirectToPage(new { id = id });
        }


    }

}
