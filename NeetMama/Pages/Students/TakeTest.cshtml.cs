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

        private async Task UpdateWeakTopicAsync(
            string studentEmail,
            string subject,
            string topic,
            bool isCorrect)
        {
            var weakTopic = await _context.StudentWeakTopics
                .FirstOrDefaultAsync(w =>
                    w.StudentEmail == studentEmail &&
                    w.Subject == subject &&
                    w.Topic == topic);

            if (weakTopic == null)
            {
                weakTopic = new StudentWeakTopic
                {
                    StudentEmail = studentEmail,
                    Subject = subject,
                    Topic = topic,
                    IncorrectCount = isCorrect ? 0 : 1,
                    TotalAttempts = 1,
                    Accuracy = isCorrect ? 100 : 0,
                    LastUpdated = DateTime.Now
                };

                _context.StudentWeakTopics.Add(weakTopic);
            }
            else
            {
                weakTopic.TotalAttempts++;

                if (!isCorrect)
                {
                    weakTopic.IncorrectCount++;
                }

                int correctCount =
                    weakTopic.TotalAttempts - weakTopic.IncorrectCount;

                weakTopic.Accuracy =
                    Math.Round((double)correctCount / weakTopic.TotalAttempts * 100, 2);

                weakTopic.LastUpdated = DateTime.Now;
            }
        }

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
                await UpdateWeakTopicAsync(
                    User.Identity?.Name ?? string.Empty,
                    question.Subject,
                    question.Topic,
                    isCorrect);

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