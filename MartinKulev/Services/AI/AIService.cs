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
                    text = "You are an AI audio classifier that finds information about the genre of a specific song.\r\n\r\nYour task is to classify each song into:\r\n- exactly ONE main genre\r\n- exactly ONE subgenre\r\n\r\nYou have to choose genres from the list below, but feel free to classify a song as another genre, if you feel like the song belongs there.\r\nYou have to find information for each song seperately. Don't overclassify a song by a genre, just because of the artist's genre.\r\n\r\n-----------------------------------\r\nINPUT FORMAT\r\n-----------------------------------\r\n\r\nI will send a JSON in the following format:\r\n\r\npublic class AISongGenreRequest\r\n{\r\n    public List<AISongGenreRequestSongInfo> Songs { get; set; }\r\n}\r\n\r\npublic class AISongGenreRequestSongInfo\r\n{\r\n    public string Title { get; set; }\r\n    public string Artist { get; set; }\r\n}\r\n\r\n-----------------------------------\r\nOUTPUT FORMAT\r\n-----------------------------------\r\n\r\nReturn a JSON in the following format ONLY:\r\n\r\npublic class AISongGenreResponse\r\n{\r\n    public List<AISongGenreResponseSongInfo> Songs { get; set; }\r\n}\r\n\r\npublic class AISongGenreResponseSongInfo\r\n{\r\n    public string Title { get; set; }\r\n    public string Artist { get; set; }\r\n    public string Genre { get; set; }\r\n    public string Subgenre { get; set; }\r\n}\r\n\r\nDo NOT include any text outside the JSON.\r\n\r\n-----------------------------------\r\n-----------------------------------\r\nGENRE LIST\r\n-----------------------------------\r\n\r\n1) Rock\r\nRock & Roll\r\nClassic Rock\r\nHard Rock\r\nSoft Rock\r\nBlues Rock\r\nGarage Rock\r\nPsychedelic Rock\r\nProgressive Rock\r\nSurf Rock\r\nFolk Rock\r\nIndie Rock\r\nAlternative Rock\r\nPost‑Rock\r\nMath Rock\r\nExperimental Rock\r\nElectronic Rock\r\nGrunge\r\nPost‑Grunge\r\nNoise Rock\r\nRap Rock\r\nSpace Rock\r\n\r\n2) Metal\r\nHeavy Metal\r\nSpeed Metal\r\nThrash Metal\r\nGroove Metal\r\nPower Metal\r\nDoom Metal\r\nBlack Metal\r\nDeath Metal\r\nSymphonic Metal\r\nProgressive Metal\r\nNu‑Metal\r\nAlternative Metal\r\nDjent\r\nMetalcore\r\nDeathcore\r\nPost‑Metal\r\nIndustrial Metal\r\nSludge Metal\r\n\r\n3) Pop\r\nMainstream Pop\r\nDance Pop\r\nSynth Pop\r\nElectropop\r\nIndie Pop\r\nAlternative Pop\r\nArt Pop\r\nPop Rock\r\nElectro‑Pop Rock\r\nPop Rap\r\n\r\n4) Hip‑Hop / Rap\r\nBoom Bap\r\nTrap\r\nDrill\r\nCloud Rap\r\nAlternative Hip‑Hop\r\nExperimental Hip‑Hop\r\nIndustrial Hip‑Hop\r\n\r\n5) Electronic\r\nHouse\r\nTechno\r\nTrance\r\nDrum & Bass\r\nDubstep\r\nBreakbeat\r\nBass Music\r\nAmbient\r\nDowntempo\r\nChillout\r\nIDM (Intelligent Dance Music)\r\nGlitch\r\nSynthwave / Retrowave\r\nFuture Bass\r\n\r\n6) R&B / Soul\r\nContemporary R&B\r\nNeo‑Soul\r\nAlternative R&B\r\nElectronic R&B\r\nQuiet Storm\r\nFunk‑Soul\r\n\r\n7) Jazz\r\nSwing\r\nBebop\r\nHard Bop\r\nCool Jazz\r\nModal Jazz\r\nFree Jazz\r\nJazz Fusion\r\nAcid Jazz\r\nSmooth Jazz\r\nAvant‑Garde Jazz\r\n\r\n8) Blues\r\nDelta Blues\r\nElectric Blues\r\nCountry Blues\r\nPiedmont Blues\r\n\r\n9) Folk / Acoustic\r\nTraditional Folk\r\nContemporary Folk\r\nIndie Folk\r\nSinger‑Songwriter Acoustic\r\nFolk Fusion\r\n\r\n10) Classical / Orchestral\r\nBaroque\r\nClassical Period\r\nRomantic\r\nModern Classical\r\nNeo‑Classical\r\nMinimalism\r\nOrchestral\r\nCinematic / Film Score\r\nAvant‑Garde Classical\r\n\r\n11) Punk\r\nPunk Rock\r\nHardcore Punk\r\nPost‑Punk\r\nPop Punk\r\nSkate Punk\r\nGarage Punk\r\nPost‑Hardcore\r\n\r\n12) Industrial / Noise\r\nIndustrial\r\nPower Electronics\r\nNoise Music\r\nExperimental Noise\r\nDark Ambient\r\n\r\n13) Reggae\r\nRoots Reggae\r\nDub\r\nDancehall\r\nRocksteady\r\nReggae Fusion\r\n\r\n14) Funk\r\nClassic Funk\r\nElectro‑Funk\r\nP‑Funk\r\nFunk Rock\r\n\r\n15) Disco\r\nClassic Disco\r\nNu‑Disco\r\nDisco House\r\nPost‑Disco\r\n\r\n16) Gospel\r\nTraditional Gospel\r\nContemporary Gospel\r\nUrban Gospel\r\nGospel Soul\r\n\r\n17) World / Traditional\r\nTraditional Percussive\r\nTraditional Melodic\r\nWorldbeat\r\nEthnic Fusion\r\nFolk Fusion"
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
