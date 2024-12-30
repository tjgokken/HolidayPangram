using HolidayPangram.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HolidayPangram.Web.Pages;

public class IndexModel(IPangramService pangramService) : PageModel
{
    public string CurrentPangram { get; private set; } = "";
    public string CurrentTheme { get; private set; } = "";

    public async Task OnGetAsync(string? theme)
    {
        // Set theme based on parameter or date
        if (!string.IsNullOrEmpty(theme))
        {
            CurrentTheme = theme;
        }
        else
        {
            var now = DateTime.Now;
            var christmas = new DateTime(now.Year, 12, 25);
            var easter = new DateTime(now.Year, 4, 9);

            CurrentTheme = now.Date > christmas.Date
                ? "christmas"
                : now.Date > easter.Date
                    ? "christmas"
                    : "easter";
        }

        CurrentPangram = await pangramService.GeneratePangram(CurrentTheme);
    }
}