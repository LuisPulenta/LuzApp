using LuzApp.Common.Responses;
using LuzApp.Common.Services;
using LuzApp.Prism.ViewModels;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;


namespace LuzApp.Prism.Views
{
    public partial class LuminariesMapPage : ContentPage
    {
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;

        public LuminariesMapPage(IGeolocatorService geolocatorService, IApiService apiService)
        {
            InitializeComponent();
            _geolocatorService = geolocatorService;
            _apiService = apiService;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MoveMapToCurrentPositionAsync();
            //LoadLuminariesAsync();
        }

        //private async void LoadLuminariesAsync()
        //{
        //    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
        //    {
        //        return;
        //    }

        //    string url = App.Current.Resources["UrlAPI"].ToString();
        //    Response response = await _apiService.GetListAsync<UserResponse>(url, "api", "/Account/GetUsers");

        //    if (!response.IsSuccess)
        //    {
        //        return;
        //    }

        //    List<UserResponse> users = (List<LuminariesResponse>)response.Result;
        //    foreach (UserResponse user in users)
        //    {
        //        MyMap.Pins.Add(new Pin
        //        {
        //            Address = user.Address,
        //            Label = user.FullName,
        //            Position = new Position(user.Latitude / 10000, user.Logitude / 10000),
        //            Type = PinType.Place
        //        });
        //    }
        //}


        private async void MoveMapToCurrentPositionAsync()
        {
            bool isLocationPermision = await CheckLocationPermisionsAsync();

            if (isLocationPermision)
            {
                MyMap.IsShowingUser = true;

                await _geolocatorService.GetLocationAsync();
                if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
                {
                    Position position = new Position(
                        _geolocatorService.Latitude,
                        _geolocatorService.Longitude);
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                        position,
                        Distance.FromKilometers(.5)));
                }
            }
        }

        private async Task<bool> CheckLocationPermisionsAsync()
        {
            Plugin.Permissions.Abstractions.PermissionStatus permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            Plugin.Permissions.Abstractions.PermissionStatus permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            Plugin.Permissions.Abstractions.PermissionStatus permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            bool isLocationEnabled = permissionLocation == Plugin.Permissions.Abstractions.PermissionStatus.Granted ||
                                        permissionLocationAlways == Plugin.Permissions.Abstractions.PermissionStatus.Granted ||
                                        permissionLocationWhenInUse == Plugin.Permissions.Abstractions.PermissionStatus.Granted;
            if (isLocationEnabled)
            {
                return true;
            }

            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

            permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            return permissionLocation == Plugin.Permissions.Abstractions.PermissionStatus.Granted ||
                    permissionLocationAlways == Plugin.Permissions.Abstractions.PermissionStatus.Granted ||
                    permissionLocationWhenInUse == Plugin.Permissions.Abstractions.PermissionStatus.Granted;
        }

    }
}
