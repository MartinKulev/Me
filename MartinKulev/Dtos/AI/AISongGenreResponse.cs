namespace MartinKulev.Dtos.AI
{
    public class AISongGenreResponse
    {
        public List<AISongGenreResponseSongInfo> Songs { get; set; }
    }

    public class AISongGenreResponseSongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Genre { get; set; }
        public string Subgenre { get; set; }
    }
    
}
