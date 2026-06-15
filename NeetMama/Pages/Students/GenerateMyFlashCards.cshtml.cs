using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;
using NeetMama.Services;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class GenerateMyFlashCardsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public string Message { get; set; } = "";

        public GenerateMyFlashCardsModel(
            ApplicationDbContext context,
            AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string studentEmail =
                User.Identity?.Name ?? "";

            var weakTopic = await _context.StudentWeakTopics
                .Where(x => x.StudentEmail == studentEmail)
                .OrderBy(x => x.Accuracy)
                .FirstOrDefaultAsync();

            if (weakTopic == null)
            {
                Message = "No weak topics found.";
                return Page();
            }

            AIFlashCardResponse? result;

            try
            {
                result = await _aiService.GenerateFlashCardsAsync(
                    weakTopic.Subject,
                    weakTopic.Topic,
                    "Concept",
                    10);
            }
            catch (Exception ex)
            {
                Message = $"AI Service Error: {ex.Message}";
                return Page();
            }

            if (result == null ||
                !result.Success ||
                result.FlashCards.Count == 0)
            {
                Message = "AI failed to generate flash cards.";
                return Page();
            }

            foreach (var card in result.FlashCards)
            {
                _context.FlashCards.Add(new FlashCard
                {
                    Subject = weakTopic.Subject,
                    Chapter = weakTopic.Topic,
                    Topic = weakTopic.Topic,
                    FrontText = card.FrontText,
                    BackText = card.BackText,
                    CardType = card.CardType,
                    CreatedBy = "AI",
                    StudentEmail = studentEmail,
                    CreatedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            Message =
                $"{result.FlashCards.Count} personalized flash cards generated for topic: {weakTopic.Topic}";

            return Page();
        }
    }
}