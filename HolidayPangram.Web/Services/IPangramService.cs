namespace HolidayPangram.Web.Services;

public interface IPangramService
{
    Task<string> GeneratePangram(string theme);
}