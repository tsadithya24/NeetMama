using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NeetMama.Data;
using NeetMama.Models;
using NeetMama.Services;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class AIGenerateFlashCardsModel : PageModel
    {
        private readonly AIService _aiService;
        private readonly ApplicationDbContext _context;

        public AIGenerateFlashCardsModel(
            AIService aiService,
            ApplicationDbContext context)
        {
            _aiService = aiService;
            _context = context;
        }

        [BindProperty]
        public AIFlashCardRequest Input { get; set; } = new();

        public string Message { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Subject) ||
                string.IsNullOrWhiteSpace(Input.Topic) ||
                Input.Count <= 0)
            {
                Message = "Please fill all fields correctly.";
                return Page();
            }

            var result = await _aiService.GenerateFlashCardsAsync(
                Input.Subject,
                Input.Topic,
                Input.CardType,
                Input.Count);

            if (result == null || !result.Success || result.FlashCards.Count == 0)
            {
                Message = "AI failed to generate flash cards.";
                return Page();
            }

            foreach (var card in result.FlashCards)
            {
                _context.FlashCards.Add(new FlashCard
                {
                    Subject = Input.Subject,
                    Chapter = Input.Topic,
                    Topic = Input.Topic,
                    FrontText = card.FrontText,
                    BackText = card.BackText,
                    CardType = card.CardType,
                    CreatedDate = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            Message = $"{result.FlashCards.Count} flash cards saved successfully.";

            return Page();
        }
    }
}