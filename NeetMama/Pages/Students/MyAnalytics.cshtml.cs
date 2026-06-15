using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class MyAnalyticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyAnalyticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<StudentWeakTopic> WeakTopics { get; set; }
            = new List<StudentWeakTopic>();

        public async Task OnGetAsync()
        {
            string studentEmail =
                User.Identity?.Name ?? "";

            WeakTopics = await _context.StudentWeakTopics
                .Where(x => x.StudentEmail == studentEmail)
                .OrderBy(x => x.Accuracy)
                .ToListAsync();
        }
    }
}