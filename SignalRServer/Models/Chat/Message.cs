using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace SignalRServer.Models.Chat;

public partial class Message
{
    public string Sender { get; set; }
    public string Text { get; set; }
    public DateTime Timestamp { get; set; }

    private static HashSet<string> BannedWords = new HashSet<string>();
    private static bool IsBannedWordsLoaded = false;
    private static readonly string BannedWordsUrl = "https://www.cs.cmu.edu/~biglou/resources/bad-words.txt";

    public Message(string sender, string text)
    {
        Sender = sender;
        Text = text;
        Timestamp = DateTime.UtcNow;
    }

    public async Task<bool> SendMessageAsync(IClientProxy client)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return false;

        // Load banned words list from online URL if not already loaded
        if (!IsBannedWordsLoaded)
        {
            await LoadBannedWordsAsync();
        }

        Text = CensorBannedWords(Text);

        await client.SendAsync("ReceiveMessage", new
        {
            sender = Sender,
            text = Text,
            timestamp = Timestamp
        });

        return true;
    }

    private static async Task LoadBannedWordsAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(BannedWordsUrl);
            BannedWords = [.. response.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim().ToLowerInvariant())];
            IsBannedWordsLoaded = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load banned words: {ex.Message}");
            BannedWords = []; // fallback: empty set
        }
    }

    private static string CensorBannedWords(string text)
    {
        return Regex.Replace(text, @"\b\w+\b", match =>
        {
            var word = match.Value;
            if (BannedWords.Contains(word.ToLowerInvariant()))
            {
                return new string('*', word.Length);
            }
            return word;
        });
    }
}
