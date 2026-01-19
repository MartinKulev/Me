namespace MartinKulev.Dtos.Music
{
    public class CurrentSongDto
    {
        public string Artist { get; set; } = "Loading...";
        public string Title { get; set; } = "Loading...";
        public string AlbumImageUrl { get; set; } = "";
        public TimeSpan Duration { get; set; } = TimeSpan.Zero; // use real duration if available
        public TimeSpan Progress { get; set; } = TimeSpan.Zero; // elapsed time
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
        public bool NowPlaying { get; set; } = false;
        public string Genre { get; set; } = "";
    }

}
