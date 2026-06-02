using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class TakeTestModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public int DurationMinutes { get; set; }
        public TakeTestModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Test? Test { get; set; }

        public IList<Question> Questions { get; set; } = new List<Question>();

        [BindProperty]
        public Dictionary<int, string> Answers { get; set; } = new Dictionary<int, string>();

        [BindProperty]
        public int DurationTakenSeconds { get; set; }

        [BindProperty]
        public bool SubmittedByTimer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id && t.IsPublished);

            if (Test == null)
            {
                return RedirectToPage("/Students/AvailableTests");
            }

            var questionIds = await _context.TestQuestions
                .Where(tq => tq.TestId == id)
                .Select(tq => tq.QuestionId)
                .ToListAsync();

            Questions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            DurationMinutes = Test.DurationMinutes;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == id && t.IsPublished);

            if (test == null)
            {
                return RedirectToPage("/Students/AvailableTests");
            }

            var questionIds = await _context.TestQuestions
                .Where(tq => tq.TestId == id)
                .Select(tq => tq.QuestionId)
                .ToListAsync();

            var questions = await _context.Questions
                .Where(q => questionIds.Contains(q.Id))
                .ToListAsync();

            int score = 0;

            var endTime = DateTime.Now;
            var startTime = endTime.AddSeconds(-DurationTakenSeconds);

            var attempt = new StudentTestAttempt
            {
                StudentEmail = User.Identity?.Name ?? string.Empty,
                TestId = id,
                StartTime = startTime,
                EndTime = endTime,
                DurationTakenSeconds = DurationTakenSeconds,
                SubmittedByTimer = SubmittedByTimer,
                TotalQuestions = questions.Count
            };

            _context.StudentTestAttempts.Add(attempt);
            await _context.SaveChangesAsync();

            foreach (var question in questions)
            {
                string selectedAnswer = "";

                if (Answers.ContainsKey(question.Id))
                {
                    selectedAnswer = Answers[question.Id];
                }

                bool isCorrect = selectedAnswer == question.CorrectAnswer;

                if (isCorrect)
                {
                    score++;
                }

                _context.StudentAnswers.Add(new StudentAnswer
                {
                    AttemptId = attempt.Id,
                    QuestionId = question.Id,
                    SelectedAnswer = selectedAnswer,
                    IsCorrect = isCorrect
                });
            }

            attempt.Score = score;

            await _context.SaveChangesAsync();

            return RedirectToPage("/Students/Results");
        }
    }
}