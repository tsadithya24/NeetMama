using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class AvailableTestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AvailableTestsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Test> Tests { get; set; } = new List<Test>();

        public async Task OnGetAsync()
        {
            Tests = await _context.Tests
                .Where(t => t.IsPublished)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}