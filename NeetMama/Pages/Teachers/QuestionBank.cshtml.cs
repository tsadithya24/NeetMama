using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;
using Microsoft.AspNetCore.Authorization;


namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class QuestionBankModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public QuestionBankModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Question> Questions { get; set; } = new List<Question>();

        public async Task OnGetAsync()
        {
            Questions = await _context.Questions
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }
    }
}