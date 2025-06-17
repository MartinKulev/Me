using MartinKulev.Dtos.Music;

namespace MartinKulev.Services.Music
{
    public class MusicService : IMusicService
    {
        public CurrentSongDto GetCurrentSong()
        {
            return new CurrentSongDto();
        }
    }
}
