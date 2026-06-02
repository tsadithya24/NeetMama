using Microsoft.AspNetCore.Mvc.RazorPages;
using NeetMama.Services;

namespace NeetMama.Pages.Admin
{
    public class AIHealthModel : PageModel
    {
        private readonly AIService _aiService;

        public AIHealthModel(AIService aiService)
        {
            _aiService = aiService;
        }

        public string Result { get; set; } = "";

        public async Task OnGetAsync()
        {
            Result = await _aiService.GetHealthAsync();
        }
    }
}