namespace MartinKulev.Data.Entities
{
    public class ListenedSong
    {
        public long Id { get; set; }
        public string Artist { get; set; }
        public string Title { get; set; }
        public string AlbumImageUrl { get; set; }
        public TimeSpan Duration { get; set; }
        public DateTime LastPlayedAt { get; set; }
        public string Genre { get; set; }
    }
}
