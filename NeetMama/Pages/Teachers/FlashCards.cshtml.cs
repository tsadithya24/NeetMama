using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
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
            FlashCards = await _context.FlashCards
                .OrderByDescending(f => f.CreatedDate)
                .ToListAsync();
        }
    }
}