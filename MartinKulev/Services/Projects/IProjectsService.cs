using MartinKulev.Dtos.Projects;

namespace MartinKulev.Services.Projects
{
    public interface IProjectsService
    {
        Task<List<GitHubRepo>> GetAllProjects();
    }
}
