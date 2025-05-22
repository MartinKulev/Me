using MartinKulev.Entities.Projects;

namespace MartinKulev.Services.Projects
{
    public interface IProjectsService
    {
        Task<List<GitHubRepo>> GetAllProjects();
    }
}
