using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace NeetMama.Pages.Teachers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
