using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var requests = Enumerable.Range(0, 1000000); // Lista de 10.000 requisições.

        var usedKeys = new ConcurrentDictionary<string, bool>();

        Parallel.ForEach(requests, new ParallelOptions { MaxDegreeOfParallelism = 10 }, async i =>
        {
            string key;

            do
            {
                key = GenerateUniqueKey();
            } while (!usedKeys.TryAdd(key, true));

            var requestBody = new StringContent(
                $"{{\"Key\":\"{key}\",\"grupo\":\"room4\"}}",
                Encoding.UTF8,
                "application/json");

            try
            {
                var response = await httpClient.PostAsync("https://fiapnet.azurewebsites.net/fiap", requestBody);
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Requisição {i}: Key = {key}, Status {response.StatusCode}, Resposta: {responseContent}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Requisição {i}: Falha com a Key = {key} - {ex.Message}");
            }
        });

        Console.WriteLine("Todas as requisições foram processadas.");
    }

    static string GenerateUniqueKey()
    {
        var random = new Random();
        char firstLetter = (char)random.Next('a', 'z' + 1);
        char lastLetter = (char)random.Next('A', 'Z' + 1);
        char digit1 = (char)random.Next('0', '9' + 1);
        char digit2 = (char)random.Next('0', '9' + 1);

        return $"{firstLetter}{digit1}{digit2}{lastLetter}";
    }
}
