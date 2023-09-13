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

                    var response = await client.GetAsync($"https://newsapi.org/v2/everything?q={_config.KeyWord}&apiKey={_config.Key}&language={_config.Language}");

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
                                    int vowelCount = CountVowels(word, _config.Language);

                                    if (vowelCount == -1)
                                        throw new Exception("Incorrect language " + _config.Language);

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
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static int CountVowels(string word, string language)
        {
            int count = 0;

            char[] vowels = Array.Empty<char>();

            if (language == "ru")
            {
                vowels = new char[] { 'а', 'е', 'ё', 'и', 'о', 'у', 'ы', 'э', 'ю', 'я' };
            }
            else if (language == "en")
            {
                vowels = new char[] { 'a', 'e', 'i', 'o', 'u' };
            }
            else
                return -1;

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