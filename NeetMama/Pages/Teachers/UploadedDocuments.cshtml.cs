using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;
using NeetMama.Services;

namespace NeetMama.Pages.Teachers
{
    public class UploadedDocumentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        private readonly AIService _aiService;

        public UploadedDocumentsModel(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            AIService aiService)
        {
            _context = context;
            _environment = environment;
            _aiService = aiService;
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

            string physicalPath =
                Path.Combine(
                    _environment.WebRootPath,
                    book.FilePath.TrimStart('/')
                        .Replace('/', Path.DirectorySeparatorChar));

            var result = await _aiService.ExtractPdfTextAsync(physicalPath);

            if (result == null || !result.Success)
            {
                TempData["ErrorMessage"] =
                    result?.Error ?? "PDF processing failed.";

                return RedirectToPage();
            }

            var existingChunks = _context.DocumentChunks
                .Where(c => c.UploadedBookId == book.Id);

            _context.DocumentChunks.RemoveRange(existingChunks);

            foreach (var chunk in result.Chunks)
            {
                _context.DocumentChunks.Add(new DocumentChunk
                {
                    UploadedBookId = book.Id,
                    ChunkNumber = chunk.Chunk_Number,
                    ChunkText = chunk.Text,
                    ChunkLength = chunk.Length,
                    CreatedDate = DateTime.Now
                });
            }

            await _aiService.StoreChunksAsync(
            book.Id,
            book.Title,
            book.Subject,
            result.Chunks);

            book.IsProcessed = true;
            book.ProcessedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                $"PDF processed successfully. Pages: {result.Page_Count}, Text length: {result.Text_Length}, Chunks: {result.Chunk_Count}";

            return RedirectToPage();
        }
    }
}