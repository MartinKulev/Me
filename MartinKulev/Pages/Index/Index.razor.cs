using MartinKulev.Services.Music;
using MartinKulev.ViewModels.Shared;
using Microsoft.AspNetCore.Components;

namespace MartinKulev.Pages.Index
{
    public partial class Index
    {
        [Inject]
        public IMusicService MusicService { get; set; }

        [Parameter]
        public SharedVm Vm { get; set; } = new SharedVm();

        protected override async Task OnInitializedAsync()
        {
            Vm.CurrentSong = MusicService.GetCurrentSong();
            MusicService.OnSongChanged += UpdateSong;
            await base.OnInitializedAsync();
        }

        private void UpdateSong()
        {
            InvokeAsync(() =>
            {
                Vm.CurrentSong = MusicService.GetCurrentSong();
                StateHasChanged(); // forces UI update
            });
        }

        public void Dispose()
        {
            MusicService.OnSongChanged -= UpdateSong;
        }
    }
}
