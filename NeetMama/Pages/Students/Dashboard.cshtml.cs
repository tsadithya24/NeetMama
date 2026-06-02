using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NeetMama.Pages.Students
{
    [Authorize(Roles = "Student,Admin")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
