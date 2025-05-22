using MartinKulev.Dtos.Projects;
using Newtonsoft.Json;

namespace MartinKulev.Services.Projects
{
    public class ProjectsService : IProjectsService
    {
        public async Task<List<GitHubRepo>> GetAllProjects()
        {
            try
            {
                string username = "MartinKulev";

                using HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                HttpResponseMessage reposResponse = await client.GetAsync($"https://api.github.com/users/{username}/repos");

                if (!reposResponse.IsSuccessStatusCode)
                    return new List<GitHubRepo>();

                string reposJson = await reposResponse.Content.ReadAsStringAsync();
                List<GitHubRepo> repos = JsonConvert.DeserializeObject<List<GitHubRepo>>(reposJson);

                return repos.Where(p => !string.IsNullOrEmpty(p.Language)).ToList();
            }
            catch(Exception ex)
            {
                return new List<GitHubRepo>();
            }
        }
    }

}
