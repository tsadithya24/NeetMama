using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Teachers
{
    public class AddQuestionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AddQuestionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Question Question { get; set; } = new Question();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Question.CreatedDate = DateTime.Now;

            _context.Questions.Add(Question);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Teachers/QuestionBank");
        }
    }
}