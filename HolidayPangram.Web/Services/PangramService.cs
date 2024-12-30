using System.Text;
using OllamaSharp;
using OllamaSharp.Models.Chat;

namespace HolidayPangram.Web.Services;

public class PangramService(IOllamaApiClient ollamaClient) : IPangramService
{
    private readonly Dictionary<string, List<string>> _fallbackPangrams = new()
    {
        ["christmas"] =
        [
            "Santa’s big fluffy reindeer joyfully zipped over quaint chimneys, making kids wish for extra presents",
            "Jolly elves baked quick gingerbread treats, wrapping festive Xmas gifts near dazzling holiday lights",
            "Jolly Santa zipped swiftly through frosty chimneys, making vibrant Xmas gifts and baking quick, warm pies"
        ],
        ["easter"] =
        [
            "Whimsical bunnies zigzagged through quiet gardens, joyfully hiding vibrant Easter eggs for excited kids to unwrap",
            "Jolly kids quickly zigzagged through meadows, spotting vibrant Easter eggs wrapped in exquisite foil",
            "Mixing bright chocolate eggs, quirky rabbits zigzagged through vibrant meadows while joyful kids unwrapped treats"
        ]
    };

    private readonly Random _random = new();

    public async Task<string> GeneratePangram(string theme)
    {
        try
        {
            var prompt = GetPrompt(theme);

            var request = new ChatRequest
            {
                Model = "llama3",
                Messages =
                [
                    new Message { Role = "user", Content = prompt }
                ]
            };

            var response = new StringBuilder();
            await foreach (var chunk in ollamaClient.ChatAsync(request))
                if (chunk?.Message?.Content != null)
                    response.Append(chunk.Message.Content);

            var pangram = response.Length > 0 ? response.ToString().Trim() : GetFallbackPangram(theme);

            // is it a proper pangram?
            return IsPangram(pangram) ? pangram : GetFallbackPangram(theme);
        }
        catch (Exception ex)
        {
            return GetFallbackPangram(theme);
        }
    }

    private static string GetPrompt(string theme)
    {
        return theme.ToLower() switch
        {
            "christmas" =>
                "Create a fun Christmas-themed pangram (sentence using every letter A-Z) about Santa, presents, elves, or holiday joy. Keep it natural and cheerful.",
            "easter" =>
                "Create a fun Easter-themed pangram (sentence using every letter A-Z) about bunnies, eggs, spring, or baskets. Keep it natural and cheerful.",
            _ => throw new ArgumentException($"Unknown theme: {theme}")
        };
    }

    private string GetFallbackPangram(string theme)
    {
        var pangrams = _fallbackPangrams[theme.ToLower()];
        return pangrams[_random.Next(pangrams.Count)];
    }

    private static bool IsPangram(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        var uniqueLetters = new HashSet<char>(text.ToLower().Where(c => c is >= 'a' and <= 'z'));

        //Console.WriteLine($"Detected letters: {string.Join(", ", uniqueLetters.OrderBy(c => c))}");

        var missingLetters = new HashSet<char>("abcdefghijklmnopqrstuvwxyz")
            .Except(uniqueLetters)
            .OrderBy(c => c)
            .ToList();

        if (missingLetters.Any())
        {
            //Console.WriteLine($"Missing letters: {string.Join(", ", missingLetters)}");
            return false;
        }

        return uniqueLetters.Count == 26;
    }
}