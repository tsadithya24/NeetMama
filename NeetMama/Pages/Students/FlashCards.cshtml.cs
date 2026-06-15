using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class FlashCardsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FlashCardsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<FlashCard> FlashCards { get; set; } = new List<FlashCard>();

        public async Task OnGetAsync()
        {
            string studentEmail =
                User.Identity?.Name ?? "";

            FlashCards = await _context.FlashCards
                .Where(x =>
                    string.IsNullOrEmpty(x.StudentEmail)
                    ||
                    x.StudentEmail == studentEmail)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }
    }
}