using MartinKulev.Services.Music;
using MartinKulev.ViewModels.Shared;
using Microsoft.AspNetCore.Components;
using System.Threading;

namespace MartinKulev.Pages.Music
{
    public partial class Music
    {
        [Inject]
        public IMusicService MusicService { get; set; }

        [Parameter]
        public SharedVm Vm { get; set; } = new SharedVm();

        private PeriodicTimer _uiTimer;

        protected override async Task OnInitializedAsync()
        {
            Vm.CurrentSong = MusicService.GetCurrentSong();

            _uiTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(250));

            _ = Task.Run(async () =>
            {
                while (await _uiTimer.WaitForNextTickAsync())
                {
                    await InvokeAsync(StateHasChanged);
                }
            });
        }

        private void UpdateSong()
        {
            InvokeAsync(() =>
            {
                Vm.CurrentSong = MusicService.GetCurrentSong();
                StateHasChanged();
            });
        }

        public void Dispose()
        {
            MusicService.OnSongChanged -= UpdateSong;
        }
    }
}
