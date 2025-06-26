using Microsoft.AspNetCore.Components;
using MartinKulev.Services.Projects;
using MartinKulev.ViewModels.Projects;
using MartinKulev.Dtos.Projects;

namespace MartinKulev.Pages.Projects
{
    public partial class Projects
    {
        [Inject]
        private IProjectsService ProjectService { get; set; }

        [Inject]
        private Serilog.ILogger Logger { get; set; }

        [Parameter]
        public ProjectsVm Vm { get; set; } = new ProjectsVm();

        protected async override Task OnInitializedAsync()
        {
            Logger.Warning("ffghhgghjhgjhgjhg");
            Console.WriteLine("lalalalalallala");
            Vm.OnSortChanged = ChangeOrderBasedOnSorting;
            Vm.GitHubRepos = await ProjectService.GetAllProjects();
            ChangeOrderBasedOnSorting(Vm.CurrentOrderOption);
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

        protected void ChangeOrderBasedOnSorting(string? selectedValue)
        {
            switch (selectedValue)
            {
                case "A-Z ↑":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderBy(r => r.Name).ToList();
                    break;
                case "Z-A ↓":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderByDescending(r => r.Name).ToList();
                    break;
                case "Created ↑":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderBy(r => r.CreatedAt).ToList();
                    break;
                case "Created ↓":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderByDescending(r => r.CreatedAt).ToList();
                    break;
                case "Last Updated ↑":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderBy(r => r.UpdatedAt).ToList();
                    break;
                case "Last Updated ↓":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderByDescending(r => r.UpdatedAt).ToList();
                    break;
                case "Language ↑":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderBy(r => r.Language).ToList();
                    break;
                case "Language ↓":
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderByDescending(r => r.Language).ToList();
                    break;
                default:
                    Vm.GitHubRepos = Vm.GitHubRepos.OrderByDescending(r => r.UpdatedAt).ToList();
                    break;
            }
        }
    }
}
