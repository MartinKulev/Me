namespace MartinKulev.Dtos.AI
{
    public class AISongGenreRequest
    {
        public List<AISongGenreRequestSongInfo> Songs { get; set; }
    }

    public class AISongGenreRequestSongInfo
    {
        public string Title { get; set; }
        public string Artist { get; set; }
    }
}
