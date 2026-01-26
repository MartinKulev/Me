namespace MartinKulev.Dtos.Music
{
    public class CurrentSongDto
    {
        public string Artist { get; set; } = "";
        public string Title { get; set; } = "";
        public string Album { get; set; } = "";
        public string AlbumImageUrl { get; set; } = "";
        public DateTime StartedAtUtc { get; set; }
        public TimeSpan StartOffset { get; set; }
        public TimeSpan Duration { get; set; } = TimeSpan.Zero; // use real duration if available
        public TimeSpan Progress { get; set; } = TimeSpan.Zero; // elapsed time
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
        public bool NowPlaying { get; set; } = false;
        public bool IsLoading { get; set; } = false;
        public string? Genre { get; set; }
        public string? SubGenre { get; set; }
        public int? ReleaseYear { get; set; }
    }

}
