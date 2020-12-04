using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using LuzApp.Common.Helpers;
using LuzApp.Common.Models;
using LuzApp.Common.Responses;
using LuzApp.Prism.ItemViewModels;
using LuzApp.Prism.Views;
using Prism.Navigation;

namespace LuzApp.Prism.ViewModels
{
    public class LuzAppMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private static LuzAppMasterDetailPageViewModel _instance;
        public static LuzAppMasterDetailPageViewModel GetInstance()
        {
            return _instance;
        }


        private UserResponse _user;
        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }


        public LuzAppMasterDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _instance = this;
            _navigationService = navigationService;
            LoadUser();
            LoadMenus();
            
        }
        public ObservableCollection<MenuItemViewModel> Menus { get; set; }


        public void LoadUser()
        {
            if (Settings.IsLogin)
            {
                
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
                User = token.User;

            }
        }


        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
        {
            new Menu
            {
                Icon = "ic_lightbulb_outline",
                PageName = $"{nameof(LuminariesPage)}",
                Title = "Luminarias"
            },
           
           
            new Menu
            {
                Icon = "ic_assignment_ind",
                PageName = $"{nameof(ModifyUserPage)}",
                Title = "ModifyUser",
                IsLoginRequired = true
            },

            new Menu
            {
                Icon = "ic_exit_to_app",
                PageName = $"{nameof(LoginPage)}",
                Title = Settings.IsLogin ? "Cerrar sesión" : "Login"
            }
        };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title,
                    IsLoginRequired = m.IsLoginRequired
                }).ToList());
        }
    }
}