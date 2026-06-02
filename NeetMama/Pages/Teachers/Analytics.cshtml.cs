using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class AnalyticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AnalyticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int TotalQuestions { get; set; }

        public int TotalTests { get; set; }

        public int PublishedTests { get; set; }

        public int UploadedDocuments { get; set; }

        public int TotalStudentAttempts { get; set; }

        public double AverageScore { get; set; }

        public List<RecentTestViewModel> RecentTests { get; set; } = new();

        public async Task OnGetAsync()
        {
            TotalQuestions = await _context.Questions.CountAsync();

            TotalTests = await _context.Tests.CountAsync();

            PublishedTests = await _context.Tests
                .CountAsync(t => t.IsPublished);

            UploadedDocuments = await _context.UploadedBooks.CountAsync();

            TotalStudentAttempts = await _context.StudentTestAttempts.CountAsync();

            var attempts = await _context.StudentTestAttempts
                .Where(a => a.TotalQuestions > 0)
                .ToListAsync();

            if (attempts.Count > 0)
            {
                AverageScore = Math.Round(
                    attempts.Average(a => (double)a.Score / a.TotalQuestions * 100),
                    2);
            }

            RecentTests = await _context.Tests
                .OrderByDescending(t => t.CreatedDate)
                .Take(5)
                .Select(t => new RecentTestViewModel
                {
                    TestName = t.TestName,
                    Subject = t.Subject,
                    DurationMinutes = t.DurationMinutes,
                    IsPublished = t.IsPublished,
                    CreatedDate = t.CreatedDate
                })
                .ToListAsync();
        }

        public class RecentTestViewModel
        {
            public string TestName { get; set; } = string.Empty;

            public string Subject { get; set; } = string.Empty;

            public int DurationMinutes { get; set; }

            public bool IsPublished { get; set; }

            public DateTime CreatedDate { get; set; }
        }
    }
}