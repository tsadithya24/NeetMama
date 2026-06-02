using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class ResultsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ResultsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ResultViewModel> Results { get; set; } = new List<ResultViewModel>();

        public async Task OnGetAsync()
        {
            string studentEmail = User.Identity?.Name ?? string.Empty;

            Results = await _context.StudentTestAttempts
                .Where(a => a.StudentEmail == studentEmail)
                .Join(
                    _context.Tests,
                    attempt => attempt.TestId,
                    test => test.Id,
                    (attempt, test) => new ResultViewModel
                    {
                        TestName = test.TestName,
                        Subject = test.Subject,
                        Score = attempt.Score,
                        TotalQuestions = attempt.TotalQuestions,
                        EndTime = attempt.EndTime,
                        AttemptId = attempt.Id,
                    })
                .OrderByDescending(r => r.EndTime)
                .ToListAsync();
        }

        public class ResultViewModel
        {
            public string TestName { get; set; } = string.Empty;

            public string Subject { get; set; } = string.Empty;

            public int Score { get; set; }

            public int TotalQuestions { get; set; }

            public DateTime EndTime { get; set; }

            public double Percentage
            {
                get
                {
                    if (TotalQuestions == 0)
                        return 0;

                    return Math.Round((double)Score / TotalQuestions * 100, 2);
                }
            }

            public int AttemptId { get; set; }
        }
    }
}