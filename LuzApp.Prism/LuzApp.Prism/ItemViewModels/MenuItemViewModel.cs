using LuzApp.Common.Helpers;
using LuzApp.Common.Models;
using LuzApp.Prism.Views;
using Prism.Commands;
using Prism.Navigation;

namespace LuzApp.Prism.ItemViewModels
{
    public class MenuItemViewModel : Menu
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectMenuCommand;

        public MenuItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectMenuCommand => _selectMenuCommand ?? (_selectMenuCommand = new DelegateCommand(SelectMenuAsync));

        private async void SelectMenuAsync()
        {
            if (PageName == nameof(LoginPage) && Settings.IsLogin)
            {
                Settings.IsLogin = false;
                Settings.Token = null;
            }

            if (IsLoginRequired && !Settings.IsLogin)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe estar logueado", "Accept");
                NavigationParameters parameters = new NavigationParameters
                    {
                        { "pageReturn", PageName }
                    };

                await _navigationService.NavigateAsync($"/{nameof(LuzAppMasterDetailPage)}/NavigationPage/{nameof(LoginPage)}", parameters);
            }
            else
            {
                await _navigationService.NavigateAsync($"/{nameof(LuzAppMasterDetailPage)}/NavigationPage/{PageName}");
            }
        }
    }
}