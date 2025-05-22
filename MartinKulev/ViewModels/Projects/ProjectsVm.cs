using MartinKulev.Dtos.Projects;

namespace MartinKulev.ViewModels.Projects
{
    public class ProjectsVm
    {
        public List<GitHubRepo> GitHubRepos { get; set; } = new List<GitHubRepo>();

        public List<string> OrderOptions = new List<string>
        {
            "A-Z ↑",
            "Z-A ↓",
            "Created ↑",
            "Created ↓",
            "Last Updated ↑",
            "Last Updated ↓",
            "Language ↑",
            "Language ↓"
        };

        private string _currentOrderOption = "Last Updated ↑";
        public string CurrentOrderOption
        {
            get => _currentOrderOption;
            set
            {
                if (_currentOrderOption != value)
                {
                    _currentOrderOption = value;
                    OnSortChanged?.Invoke(value);
                }
            }
        }

        public Action<string>? OnSortChanged { get; set; }

    }
}
