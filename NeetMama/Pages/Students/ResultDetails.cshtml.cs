using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class ResultDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResultDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<QuestionReview> Reviews { get; set; } = new();

        public async Task OnGetAsync(int id)
        {
            Reviews = await _context.StudentAnswers
                .Where(sa => sa.AttemptId == id)
                .Join(
                    _context.Questions,
                    answer => answer.QuestionId,
                    question => question.Id,
                    (answer, question) => new QuestionReview
                    {
                        QuestionText = question.QuestionText,
                        StudentAnswer = answer.SelectedAnswer,
                        CorrectAnswer = question.CorrectAnswer,
                        Explanation = question.Explanation,
                        IsCorrect = answer.IsCorrect
                    })
                .ToListAsync();
        }

        public class QuestionReview
        {
            public string QuestionText { get; set; } = string.Empty;

            public string StudentAnswer { get; set; } = string.Empty;

            public string CorrectAnswer { get; set; } = string.Empty;

            public bool IsCorrect { get; set; }

            public string Explanation { get; set; } = string.Empty;
        }
    }
}