using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeetMama.Data;
using NeetMama.Models;


namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class UploadBooksModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        [TempData]
        public string SuccessMessage { get; set; } = string.Empty;

        public UploadBooksModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public UploadedBook UploadedBook { get; set; } = new UploadedBook();

        [BindProperty]
        public IFormFile? PdfFile { get; set; }

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (PdfFile == null || PdfFile.Length == 0)
            {
                Message = "Please select a PDF file.";
                return Page();
            }

            long maxFileSize = 500L * 1024L * 1024L;

            if (PdfFile.Length > maxFileSize)
            {
                Message = "File size should not exceed 500 MB.";
                return Page();
            }

            string extension = Path.GetExtension(PdfFile.FileName).ToLower();

            if (extension != ".pdf")
            {
                Message = "Only PDF files are allowed.";
                return Page();
            }

            string webRootPath = _environment.WebRootPath;

            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string uploadsFolder = Path.Combine(webRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string safeFileName = Path.GetFileName(PdfFile.FileName);
            string uniqueFileName = $"{Guid.NewGuid()}_{safeFileName}";
            string fullFilePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                await PdfFile.CopyToAsync(stream);
            }

            UploadedBook.FileName = safeFileName;
            UploadedBook.FilePath = "/uploads/" + uniqueFileName;
            UploadedBook.UploadedDate = DateTime.Now;

            _context.UploadedBooks.Add(UploadedBook);
            await _context.SaveChangesAsync();

            SuccessMessage = "PDF uploaded successfully.";

            return RedirectToPage("/Teachers/UploadBooks");
        }
    }
}