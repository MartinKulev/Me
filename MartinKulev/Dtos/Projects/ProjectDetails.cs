namespace MartinKulev.Dtos.Projects
{
    public class ProjectDetails : GitHubRepo
    {     
        public string ReadMe { get; set; }

        public int CommitsCount { get; set; }

        public List<string> Languages { get; set; }
    }
}
