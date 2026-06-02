using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class AnalyticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalTestsTaken { get; set; }

        public double AverageScore { get; set; }

        public double HighestScore { get; set; }

        public double LowestScore { get; set; }

        public List<RecentResultViewModel> RecentResults { get; set; } = new();

        public async Task OnGetAsync()
        {
            string studentEmail = User.Identity?.Name ?? string.Empty;

            var attempts = await _context.StudentTestAttempts
                .Where(a => a.StudentEmail == studentEmail)
                .ToListAsync();

            TotalTestsTaken = attempts.Count;

            if (attempts.Count > 0)
            {
                var percentages = attempts
                    .Where(a => a.TotalQuestions > 0)
                    .Select(a => (double)a.Score / a.TotalQuestions * 100)
                    .ToList();

                AverageScore = Math.Round(percentages.Average(), 2);
                HighestScore = Math.Round(percentages.Max(), 2);
                LowestScore = Math.Round(percentages.Min(), 2);
            }

            RecentResults = await _context.StudentTestAttempts
                .Where(a => a.StudentEmail == studentEmail)
                .Join(
                    _context.Tests,
                    attempt => attempt.TestId,
                    test => test.Id,
                    (attempt, test) => new RecentResultViewModel
                    {
                        TestName = test.TestName,
                        Subject = test.Subject,
                        Score = attempt.Score,
                        TotalQuestions = attempt.TotalQuestions,
                        CompletedOn = attempt.EndTime
                    })
                .OrderByDescending(r => r.CompletedOn)
                .Take(5)
                .ToListAsync();
        }

        public class RecentResultViewModel
        {
            public string TestName { get; set; } = string.Empty;

            public string Subject { get; set; } = string.Empty;

            public int Score { get; set; }

            public int TotalQuestions { get; set; }

            public DateTime CompletedOn { get; set; }

            public double Percentage =>
                TotalQuestions == 0 ? 0 : Math.Round((double)Score / TotalQuestions * 100, 2);
        }
    }
}