using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text;
using Newtonsoft.Json;
using MartinKulev.Services.AI;

namespace MartinAI.Services
{
    public class AIService : IAIService
    {
        private readonly string? _apiKey;
        private readonly string? _apiUrl;

        public AIService(IConfiguration configuration)
        {
            _apiKey = configuration.GetValue<string>("APIKeys:OpenAI:ApiKey") ?? configuration.GetValue<string>("APIKeys:OpenAI:ApiKey");
            _apiUrl = configuration.GetValue<string>("APIKeys:OpenAI:ApiUrl");
        }

        public async Task<string> GetAgentResponse(string prompt, string instructions)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var requestBody = new
            {   
                model = "gpt-5.2",
                input = new object[]
    {
        new
        {
            role = "system",
            content = new[]
            {
                new
                {
                    type = "input_text",
                    text = instructions
                }
            }
        },
        new
        {
            role = "user",
            content = new[]
            {
                new
                {
                    type = "input_text",
                    text = prompt
                }
            }
        }
    }
            };

            var jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(_apiUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                // Agents may return structured outputs; if just text, often under 'output_text'
                if (jsonResponse.output_text != null)
                    return jsonResponse.output_text.ToString();

                // Otherwise fallback to generic content parsing
                return jsonResponse.output[0].content[0].text.ToString();
            }
            else
            {
                return $"Error: {response.StatusCode}\n{responseString}";
            }
        }
    }
}
