using Markdig;
using MartinKulev.Services.Projects;
using MartinKulev.ViewModels.Projects;
using Microsoft.AspNetCore.Components;

namespace MartinKulev.Pages.Projects
{
    public partial class ProjectDetails
    {
        [Inject]
        public IProjectsService ProjectsService { get; set; }

        [Parameter]
        public ProjectDetailsVm Vm { get; set; } = new ProjectDetailsVm();

        [Parameter]
        public string? ProjectName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Vm.ProjectDetails = await ProjectsService.GetProjectDetails(ProjectName);
            Vm.ProjectDetails.ReadMe = GetReadMeHtml(Vm.ProjectDetails.ReadMe);
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

        public string GetReadMeHtml(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return "<p>No README available.</p>";

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}
