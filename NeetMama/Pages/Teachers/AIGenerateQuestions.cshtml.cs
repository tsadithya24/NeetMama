using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeetMama.Data;
using NeetMama.Models;
using NeetMama.Services;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class AIGenerateQuestionsModel : PageModel
    {
        private readonly AIService _aiService;
        private readonly ApplicationDbContext _context;

        public AIGenerateQuestionsModel(
            AIService aiService,
            ApplicationDbContext context)
        {
            _aiService = aiService;
            _context = context;
        }

        [BindProperty]
        public AIQuestionRequest Input { get; set; } = new AIQuestionRequest();

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Subject) ||
                string.IsNullOrWhiteSpace(Input.Topic) ||
                string.IsNullOrWhiteSpace(Input.Difficulty) ||
                Input.Count <= 0)
            {
                Message = "Please fill all fields correctly.";
                return Page();
            }

            var result =
                await _aiService.GenerateQuestionsAsync(
                    Input.Subject,
                    Input.Topic,
                    Input.Difficulty,
                    Input.QuestionType,
                    Input.Count);

            if (result == null || !result.Success || result.Questions.Count == 0)
            {
                Message = "AI failed to generate questions.";
                return Page();
            }

            foreach (var generated in result.Questions)
            {
                _context.Questions.Add(new Question
                {
                    Subject = Input.Subject,
                    Chapter = Input.Topic,
                    Topic = Input.Topic,
                    QuestionType = Input.QuestionType,
                    QuestionText = generated.Question,
                    OptionA = generated.OptionA,
                    OptionB = generated.OptionB,
                    OptionC = generated.OptionC,
                    OptionD = generated.OptionD,
                    CorrectAnswer = generated.CorrectAnswer,
                    Explanation = generated.Explanation,
                    Difficulty = Input.Difficulty,
                    CreatedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            Message = $"{result.Questions.Count} AI-generated questions saved to Question Bank.";

            return Page();
        }
    }
}