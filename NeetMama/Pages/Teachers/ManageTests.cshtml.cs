using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NeetMama.Data;
using NeetMama.Models;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class ManageTestsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ManageTestsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Test> Tests { get; set; } = new List<Test>();

        public async Task OnGetAsync()
        {
            Tests = await _context.Tests
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostPublishAsync(int id)
        {
            var test = await _context.Tests.FindAsync(id);

            if (test != null)
            {
                test.IsPublished = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}