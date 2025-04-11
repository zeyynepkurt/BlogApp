using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlogApp.Data;

namespace BlogApp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; }


        public void OnGet()
        {
            

        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            if (User.Identity.Name != "admin@example.com")
            {
                return Forbid(); // 👈 sadece admin erişebilir
            }

            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("🟡 Form gönderildi"); // LOG 1
            Console.WriteLine("Gelen ad: " + Category?.Name);


            if (!ModelState.IsValid)
            {
                Console.WriteLine("🔴 Model geçersiz"); // LOG 2
                return Page();
            }
            if (User.Identity.Name != "admin@example.com")
            {
                return Forbid(); // 👈 sadece admin erişebilir
            }

            Console.WriteLine("🟢 Model geçerli: " + Category.Name); // LOG 3

            _context.Categories.Add(Category);
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Veri kaydedildi"); // LOG 4

            return RedirectToPage("./Index");
        }
    }
}
