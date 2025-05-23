using MartinKulev.Dtos.Projects;
using Newtonsoft.Json;

namespace MartinKulev.Services.Projects
{
    public class ProjectsService : IProjectsService
    {
        public string _username = "MartinKulev";
        public async Task<List<GitHubRepo>> GetAllProjects()
        {
            try
            {
                using HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                HttpResponseMessage reposResponse = await client.GetAsync($"https://api.github.com/users/{_username}/repos");

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

        public async Task<ProjectDetails> GetProjectDetails(string repoName)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");



            var repoUrl = $"https://api.github.com/repos/{_username}/{repoName}";
            var repoResponse = await client.GetAsync(repoUrl);
            if (!repoResponse.IsSuccessStatusCode)
            {
                return null;
            }
            var repoJson = await repoResponse.Content.ReadAsStringAsync();
            var repo = JsonConvert.DeserializeObject<GitHubRepo>(repoJson);



            var readmeUrl = $"https://api.github.com/repos/{_username}/{repoName}/readme";
            var readmeResponse = await client.GetAsync(readmeUrl);
            string readmeContent = "";
            if (readmeResponse.IsSuccessStatusCode)
            {
                var readmeJson = await readmeResponse.Content.ReadAsStringAsync();
                var readmeData = JsonConvert.DeserializeObject<dynamic>(readmeJson);
                string downloadUrl = readmeData.download_url;

                var rawReadme = await client.GetAsync(downloadUrl);
                if (rawReadme.IsSuccessStatusCode)
                {
                    readmeContent = await rawReadme.Content.ReadAsStringAsync();
                }
            }



            var commitsUrl = $"https://api.github.com/repos/{_username}/{repoName}/commits";
            var commitsResponse = await client.GetAsync(commitsUrl);
            int commitsCount = 0;
            if (commitsResponse.IsSuccessStatusCode)
            {
                var commitsJson = await commitsResponse.Content.ReadAsStringAsync();
                var commits = JsonConvert.DeserializeObject<List<object>>(commitsJson);
                commitsCount = commits?.Count ?? 0;
            }

            string languagesUrl = $"https://api.github.com/repos/{_username}/{repoName}/languages";
            var languagesResponse = await client.GetAsync(languagesUrl);
            List<string> languagesList = new List<string>();
            if (languagesResponse.IsSuccessStatusCode)
            {
                var json = await languagesResponse.Content.ReadAsStringAsync();
                var languages = JsonConvert.DeserializeObject<Dictionary<string, long>>(json);
                languagesList = languages.Keys.ToList();
            }

            return new ProjectDetails
            {
                Name = repo.Name,
                HtmlUrl = repo.HtmlUrl,
                Homepage = repo.Homepage,
                Language = repo.Language,
                CreatedAt = repo.CreatedAt,
                UpdatedAt = repo.UpdatedAt,
                CommitsCount = commitsCount,
                Languages = languagesList,
                ReadMe = readmeContent
            };
        }
    }

}
