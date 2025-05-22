using MartinKulev.Entities.Projects;

namespace MartinKulev.ViewModels.Projects
{
    public class ProjectsVm
    {
        public List<GitHubRepo> GitHubRepos { get; set; } = new List<GitHubRepo>();
    }
}
