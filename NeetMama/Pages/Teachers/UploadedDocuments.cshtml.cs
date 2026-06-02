using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Teachers
{
    public class UploadedDocumentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public UploadedDocumentsModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IList<UploadedBook> UploadedBooks { get; set; }
            = new List<UploadedBook>();

        public async Task OnGetAsync()
        {
            UploadedBooks = await _context.UploadedBooks
                .OrderByDescending(x => x.UploadedDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var book = await _context.UploadedBooks
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return RedirectToPage();
            }

            string physicalPath =
                Path.Combine(
                    _environment.WebRootPath,
                    book.FilePath.TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _context.UploadedBooks.Remove(book);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostProcessAsync(int id)
        {
            var book = await _context.UploadedBooks
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return RedirectToPage();
            }

            book.IsProcessed = true;
            book.ProcessedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}