using Microsoft.AspNetCore.Components;
using MartinKulev.Services.Projects;
using MartinKulev.ViewModels.Projects;

namespace MartinKulev.Pages.Projects
{
    public partial class Projects
    {
        [Inject]
        private IProjectsService ProjectService { get; set; }

        [Parameter]
        public ProjectsVm Vm { get; set; } = new ProjectsVm();

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Vm.GitHubRepos = await ProjectService.GetAllProjects();
        }

        protected string GetLanguageClass(string language)
        {
            return language?.ToLowerInvariant() switch
            {
                "c#" => "csharp",
                "c++" => "cpp",
                "f#" => "fsharp",
                _ => language?.ToLowerInvariant()?.Replace(" ", "-")
            };
        }
    }
}
