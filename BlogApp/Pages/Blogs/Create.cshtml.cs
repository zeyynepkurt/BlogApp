using BlogApp.Models;
using BlogApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Pages.Blogs
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CreateModel(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }
        [BindProperty]
        public Blog Blog { get; set; } = new Blog();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        private readonly IWebHostEnvironment _environment;

        

        public void OnGet()
        {
            // 👇 Burası eksikti, bu satır dropdown verisini gönderiyor
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("📝 Title: " + Blog?.Title);
            Console.WriteLine("📝 Content: " + Blog?.Content);
            Console.WriteLine("📝 Author: " + Blog?.Author);
            Console.WriteLine("📝 CategoryId: " + Blog?.CategoryId);

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ ModelState geçersiz!");

                foreach (var state in ModelState)
                {
                    if (state.Value.Errors.Count > 0)
                    {
                        Console.WriteLine($"🔴 {state.Key}: {state.Value.Errors[0].ErrorMessage}");
                    }
                }

                ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return Page();
            }

            if (ImageFile != null)
            {
                // Dosya adını oluştur (benzersiz olsun)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                // uploads klasörü yoksa oluştur
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Blog modeline yolu kaydet
                Blog.ImagePath = "/uploads/" + fileName;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            Blog.UserId = currentUser.Id;

            Blog.PublishDate = DateTime.Now;
            

            _context.Blogs.Add(Blog);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Blog başarıyla eklendi.");
            return RedirectToPage("./Index");
        }

    }
}
