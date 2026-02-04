using MartinKulev.Dtos.Music;

namespace MartinKulev.Services.Music
{
    public interface IMusicService
    {
        CurrentSongDto GetCurrentSong();

        event Action OnSongChanged;
    }
}
