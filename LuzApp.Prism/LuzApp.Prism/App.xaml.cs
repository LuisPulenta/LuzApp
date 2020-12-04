using Prism;
using Prism.Ioc;
using LuzApp.Prism.ViewModels;
using LuzApp.Prism.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using LuzApp.Prism.Helpers;
using LuzApp.Common.Services;
using LuzApp.Common.Helpers;

namespace LuzApp.Prism
{
    public partial class App
    {
        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTY2MzIyQDMxMzcyZTMzMmUzMFVnNW5KSnM2dTZmRDljWm1RYTduQXFwRmNKSzVPWk1lT1JGSFRySXZCUTA9");
            InitializeComponent();

            if (Settings.IsLogin)
            {
                await NavigationService.NavigateAsync("/LuzAppMasterDetailPage/NavigationPage/LuminariesPage");
            }

            else
            {
                await NavigationService.NavigateAsync("/LuzAppMasterDetailPage/NavigationPage/LoginPage"); ;
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.Register<IApiService, ApiService>();
            containerRegistry.Register<IGeolocatorService, GeolocatorService>();
            containerRegistry.Register<IRegexHelper, RegexHelper>();
            
            

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<LoginPage, LoginPageViewModel>();
            containerRegistry.RegisterForNavigation<RecoverPasswordPage, RecoverPasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<RegisterPage, RegisterPageViewModel>();
            containerRegistry.RegisterForNavigation<LuzAppMasterDetailPage, LuzAppMasterDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<ModifyUserPage, ModifyUserPageViewModel>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage, ChangePasswordPageViewModel>();
            containerRegistry.RegisterForNavigation<LuminariesPage, LuminariesPageViewModel>();
            containerRegistry.RegisterForNavigation<LuminariesMapPage, LuminariesMapPageViewModel>();
            containerRegistry.RegisterForNavigation<AddLuminaryPage, AddLuminaryPageViewModel>();
            containerRegistry.RegisterForNavigation<TakePhotoPage, TakePhotoPageViewModel>();
        }
    }
}
