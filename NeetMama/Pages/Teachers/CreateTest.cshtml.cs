using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class CreateTestModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateTestModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Test Test { get; set; } = new Test();

        [BindProperty]
        public List<int> SelectedQuestionIds { get; set; } = new List<int>();

        public IList<Question> Questions { get; set; } = new List<Question>();

        public async Task OnGetAsync()
        {
            Questions = await _context.Questions
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Questions = await _context.Questions
                .OrderByDescending(q => q.CreatedDate)
                .ToListAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (SelectedQuestionIds == null || SelectedQuestionIds.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select at least one question.");
                return Page();
            }

            Test.CreatedDate = DateTime.Now;
            Test.IsPublished = false;

            _context.Tests.Add(Test);
            await _context.SaveChangesAsync();

            foreach (var questionId in SelectedQuestionIds)
            {
                _context.TestQuestions.Add(new TestQuestion
                {
                    TestId = Test.Id,
                    QuestionId = questionId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Teachers/QuestionBank");
        }
    }
}