using Prism.Commands;
using Prism.Navigation;

namespace LuzApp.Prism.ViewModels
{
    public class LuminariesPageViewModel : ViewModelBase
    {
        private DelegateCommand _addLuminaryCommand;
        public DelegateCommand AddLuminaryCommand => _addLuminaryCommand ?? (_addLuminaryCommand = new DelegateCommand(AddLuminary));

        private DelegateCommand _luminariesMapCommand;
        public DelegateCommand LuminariesMapCommand => _luminariesMapCommand ?? (_luminariesMapCommand = new DelegateCommand(LuminariesMap));

        private readonly INavigationService _navigationService;

        public LuminariesPageViewModel(INavigationService navigationService) : base(navigationService)
        {

            Title = "Luminarias";
            _navigationService = navigationService;
        }
        private async void AddLuminary()
        {
            await _navigationService.NavigateAsync("AddLuminaryPage");
        }

        private async void LuminariesMap()
        {
            await _navigationService.NavigateAsync("LuminariesMapPage");
        }
    }
}