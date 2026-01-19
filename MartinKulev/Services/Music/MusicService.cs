using MartinKulev.Data;
using MartinKulev.Data.Entities;
using MartinKulev.Dtos.Music;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MartinKulev.Services.Music
{
    public class MusicService : IMusicService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpClient _httpClient;
        private readonly string API_KEY;
        private readonly string SHARED_SECRET;
        private readonly string USERNAME;
        private string _sessionKey;
        private Timer _trackTimer;   // updates progress every second
        private Timer _fetchTimer;   // fetches new track every 1s
        private Timer _addUnexstingTrackToDbTimer;   // adds an item if it doesn't exist every 1min
        private Timer _fetchAllSongsTimer;   // fetches all songs every 10min

        private CurrentSongDto _currentSong;

        public event Action OnSongChanged; // Notify UI
        private ConcurrentBag<ListenedSong> _listenedSongsCache = new ConcurrentBag<ListenedSong>();

        public MusicService(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            API_KEY = configuration.GetValue<string>("APIKeys:LastFM:ApiKey")!;
            SHARED_SECRET = configuration.GetValue<string>("APIKeys:LastFM:SharedSecret")!;
            USERNAME = configuration.GetValue<string>("APIKeys:LastFM:Username")!;
            _httpClient = new HttpClient();
            FetchAllSongsFromDbToCache().GetAwaiter().GetResult();
            _currentSong = new CurrentSongDto
            {
                Artist = "Loading...",
                Title = "Loading...",
                IsLoading = true
            };

            _trackTimer = new Timer(_ =>
            {
                if (_currentSong.Progress < _currentSong.Duration)
                {
                    _currentSong.Progress = _currentSong.Progress.Add(TimeSpan.FromSeconds(1));
                    OnSongChanged?.Invoke();
                }
            }, null, 0, 950);

            _fetchTimer = new Timer(async _ =>
            {
                await FetchCurrentSong();
                OnSongChanged?.Invoke();
            }, null, 0, 1000);

            _addUnexstingTrackToDbTimer = new Timer(async _ =>
            {
                await AddCurrentSongToDb();
                OnSongChanged?.Invoke();
            }, null, 0, 1000 * 60);
            
            _fetchAllSongsTimer = new Timer(async _ =>
            {
                await FetchAllSongsFromDbToCache();
                OnSongChanged?.Invoke();
            }, null, 0, 1000 * 60 * 10);

        }

        public CurrentSongDto GetCurrentSong() => _currentSong;

        private async Task FetchCurrentSong()
        {
            try
            {
                var recentTrack = await GetLastListenedTrackDto(_sessionKey);
                _currentSong.IsLoading = false;
                _currentSong.Genre = recentTrack.Genre;
                Log.Logger.Warning($"{DateTime.UtcNow}: {recentTrack.Artist} - {recentTrack.Title}, {recentTrack?.Genre}");

                if (_currentSong.Title != recentTrack.Title || _currentSong.Artist != recentTrack.Artist)
                {
                    _currentSong = recentTrack;

                    OnSongChanged?.Invoke();
                }
                else
                {
                    if (_currentSong.NowPlaying)
                    {
                        _currentSong.Progress = DateTime.UtcNow - _currentSong.PlayedAt;
                        OnSongChanged?.Invoke();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching current song: {ex.Message} {ex.StackTrace}");
            }
        }

        public async Task SetSessionKeyFromToken(string token)
        {
            var parameters = new SortedDictionary<string, string>
            {
                { "api_key", API_KEY },
                { "method", "auth.getSession" },
                { "token", token }
            };

            string apiSig = GenerateApiSignature(parameters);

            string url =
                $"https://ws.audioscrobbler.com/2.0/?" +
                $"method=auth.getSession" +
                $"&api_key={API_KEY}" +
                $"&token={token}" +
                $"&api_sig={apiSig}" +
                $"&format=json";

            var response = await _httpClient.GetStringAsync(url);

            using var json = JsonDocument.Parse(response);
            _sessionKey = json.RootElement
                       .GetProperty("session")
                       .GetProperty("key")
                       .GetString()!;
        }

        private string GenerateApiSignature(SortedDictionary<string, string> parameters)
        {
            var sb = new StringBuilder();

            foreach (var p in parameters)
                sb.Append(p.Key).Append(p.Value);

            sb.Append(SHARED_SECRET);

            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));

            return BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLowerInvariant();
        }

        private async Task<CurrentSongDto> GetLastListenedTrackDto(string sessionKey)
        {
            string recentUrl = $"https://ws.audioscrobbler.com/2.0/?method=user.getRecentTracks&user={USERNAME}&limit=1&api_key={API_KEY}&sk={sessionKey}&format=json";
            var recentResponse = await _httpClient.GetStringAsync(recentUrl);

            using var recentJson = JsonDocument.Parse(recentResponse);
            var trackElement = recentJson.RootElement
                                         .GetProperty("recenttracks")
                                         .GetProperty("track")[0];

            string artist = trackElement.GetProperty("artist").GetProperty("#text").GetString();
            string title = trackElement.GetProperty("name").GetString();

            bool nowPlaying = trackElement.TryGetProperty("@attr", out var attr) &&
                              attr.TryGetProperty("nowplaying", out var np) &&
                              np.GetString() == "true";

            DateTime playedAt;
            if (!nowPlaying && trackElement.TryGetProperty("date", out var dateProp))
            {
                if (dateProp.TryGetProperty("uts", out var utsProp))
                {
                    long unixTime = 0;
                    switch (utsProp.ValueKind)
                    {
                        case JsonValueKind.Number:
                            unixTime = utsProp.GetInt64();
                            break;
                        case JsonValueKind.String:
                            long.TryParse(utsProp.GetString(), out unixTime);
                            break;
                    }
                    playedAt = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
                }
                else
                {
                    playedAt = DateTime.UtcNow;
                }
            }
            else
            {
                playedAt = DateTime.UtcNow;
            }

            ListenedSong? listenedSong = _listenedSongsCache.FirstOrDefault(ls => ls.Artist == artist && ls.Title == title);
            TimeSpan duration = TimeSpan.Zero;
            try
            {
                string trackInfoUrl = $"https://ws.audioscrobbler.com/2.0/?method=track.getInfo&api_key={API_KEY}" +
                                      $"&artist={Uri.EscapeDataString(artist)}&track={Uri.EscapeDataString(title)}&format=json";
                var trackInfoResponse = await _httpClient.GetStringAsync(trackInfoUrl);

                using var infoJson = JsonDocument.Parse(trackInfoResponse);
                var trackInfo = infoJson.RootElement.GetProperty("track");

                if (trackInfo.TryGetProperty("duration", out var durationProp))
                {
                    switch (durationProp.ValueKind)
                    {
                        case JsonValueKind.Number:
                            duration = TimeSpan.FromMilliseconds(durationProp.GetInt64());
                            break;
                        case JsonValueKind.String:
                            if (long.TryParse(durationProp.GetString(), out var durMs))
                                duration = TimeSpan.FromMilliseconds(durMs);
                            break;
                    }
                }
            }
            catch
            {
                duration = TimeSpan.Zero;
            }

            string imageUrl = null;
            if (trackElement.TryGetProperty("image", out var trackImages))
            {
                foreach (var img in trackImages.EnumerateArray())
                {
                    if (img.GetProperty("size").GetString() == "extralarge")
                    {
                        var url = img.GetProperty("#text").GetString();
                        if (!string.IsNullOrEmpty(url))
                        {
                            imageUrl = url;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    foreach (var img in trackImages.EnumerateArray())
                    {
                        var url = img.GetProperty("#text").GetString();
                        if (!string.IsNullOrEmpty(url))
                        {
                            imageUrl = url;
                            break;
                        }
                    }
                }
            }

            //if (string.IsNullOrEmpty(imageUrl))
            //{
            //    imageUrl = await GetArtistImage(artist);
            //}

            if (string.IsNullOrEmpty(imageUrl))
            {
                imageUrl = "/MusicNote.png";
            }

            TimeSpan progress = nowPlaying ? DateTime.UtcNow - playedAt : duration;

            return new CurrentSongDto
            {
                Artist = artist,
                Title = title,
                AlbumImageUrl = imageUrl,
                Duration = duration,
                Progress = progress,
                PlayedAt = playedAt,
                NowPlaying = nowPlaying,
                Genre = listenedSong?.Genre
            };
        }

        private async Task AddCurrentSongToDb()
        {
            if(!_currentSong.IsLoading)
            {
                using MartinKulevDbContext localDbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<MartinKulevDbContext>();
                var existingSong = await localDbContext.ListenedSongs.FirstOrDefaultAsync(ls => ls.Artist == _currentSong.Artist && ls.Title == _currentSong.Title);
                if (existingSong == null)
                {
                    var newSong = new ListenedSong
                    {
                        Artist = _currentSong.Artist,
                        Title = _currentSong.Title,
                        AlbumImageUrl = _currentSong.AlbumImageUrl,
                        Duration = _currentSong.Duration,
                        LastPlayedAt = _currentSong.PlayedAt,
                        Genre = _currentSong?.Genre ?? string.Empty
                    };
                    await localDbContext.ListenedSongs.AddAsync(newSong);
                    await localDbContext.SaveChangesAsync();
                }
            }
        }

        private async Task FetchAllSongsFromDbToCache()
        {
            using MartinKulevDbContext localDbContext = _serviceProvider.CreateScope().ServiceProvider.GetService<MartinKulevDbContext>();
            _listenedSongsCache = [.. await localDbContext.ListenedSongs.AsNoTracking().ToListAsync()];
        }

        private async Task<string> GetArtistImage(string artistName)
        {
            try
            {
                string url = $"https://ws.audioscrobbler.com/2.0/?method=artist.getInfo&artist={Uri.EscapeDataString(artistName)}&api_key={API_KEY}&format=json";
                var response = await _httpClient.GetStringAsync(url);

                using var json = JsonDocument.Parse(response);
                var artist = json.RootElement.GetProperty("artist");

                foreach (var img in artist.GetProperty("image").EnumerateArray())
                {
                    if (img.GetProperty("size").GetString() == "extralarge")
                    {
                        var imageUrl = img.GetProperty("#text").GetString();
                        if (!string.IsNullOrEmpty(imageUrl))
                            return imageUrl;
                    }
                }
            }
            catch
            {
                var a = 0;
            }

            return string.Empty;
        }

    }
}

