using System.Text.Json;

namespace TestConsoleApp
{
    public class Program
    {
        private static readonly AppConfig _config = JsonSerializer.Deserialize<AppConfig>(new StreamReader("appconfig.json").ReadToEnd());
        static async Task Main(string[] args)
        {
            using (HttpClient client = new())
            {
                try
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MyTestApp");

                    var response = await client.GetAsync($"https://newsapi.org/v2/everything?q={_config.KeyWord}&apiKey={_config.Key}");

                    var responseBody = await response.Content.ReadAsStringAsync();

                    var news = JsonSerializer.Deserialize<NewsObject>(responseBody);

                    if (responseBody != null)
                    {
                        foreach (var article in news.Articles)
                        {
                            string[]? words = article.Description?.Split(' ');

                            Console.WriteLine("Фрагмент новости: \n" + article.Description);

                            string wordWithMostVowels = "";

                            int maxVowelCount = 0;

                            if (words != null)
                            {
                                foreach (string word in words)
                                {
                                    int vowelCount = CountVowels(word);
                                    if (vowelCount > maxVowelCount)
                                    {
                                        maxVowelCount = vowelCount;
                                        wordWithMostVowels = word;
                                    }
                                }
                                Console.WriteLine("Слово с наибольшим количеством гласных - " + wordWithMostVowels);
                            }
                        }
                    }


                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        public static int CountVowels(string word)
        {
            int count = 0;
            char[] vowels = { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };

            foreach (char c in word)
            {
                if (vowels.Contains(Char.ToLower(c)))
                {
                    count++;
                }
            }

            return count;
        }
    }
}