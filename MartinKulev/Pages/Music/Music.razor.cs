using MartinKulev.Services.Music;
using Microsoft.AspNetCore.Components;

namespace MartinKulev.Pages.Music
{
    public partial class Music
    {
        [Inject]
        public IMusicService MusicService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
