using LuzApp.Common.Helpers;
using LuzApp.Common.Models;
using LuzApp.Common.Requests;
using LuzApp.Common.Responses;
using LuzApp.Common.Services;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace LuzApp.Prism.ViewModels
{
    public class AddLuminaryPageViewModel : ViewModelBase
    {


        private readonly INavigationService _navigationService;
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private bool _isEnabled;
        private bool _isRefreshing1;
        private bool _isRefreshing2;

        


        private ImageSource _imageSource1;
        private ImageSource _imageSource2;
        private ImageSource _imageSource3;

        private DelegateCommand _takePhotoCommand1;
        private DelegateCommand _takePhotoCommand2;
        private DelegateCommand _takePhotoCommand3;

        private DelegateCommand _cancelCommand;
        private DelegateCommand _saveCommand;
        

        private DelegateCommand _getAddressCommand;

        private MediaFile _file1;
        private MediaFile _file2;
        private MediaFile _file3;

        private Position _position;

        private DateTime _date;

        private Estado _estado;
        public Estado Estado
        {
            get => _estado;
            set => SetProperty(ref _estado, value);
        }

        private TipoLuminaria _tLuminaria;
        public TipoLuminaria TLuminaria
        {
            get => _tLuminaria;
            set => SetProperty(ref _tLuminaria, value);
        }

        private ObservableCollection<TipoLuminaria> _tiposLuminaria;
        public ObservableCollection<TipoLuminaria> TiposLuminaria
        {
            get => _tiposLuminaria;
            set => SetProperty(ref _tiposLuminaria, value);
        }

        private ObservableCollection<Estado> _estados;
        public ObservableCollection<Estado> Estados
        {
            get => _estados;
            set => SetProperty(ref _estados, value);
        }


        private string _remarks;
        public string Remarks
        {
            get => _remarks;
            set => SetProperty(ref _remarks, value);
        }

        private string _filter;

        private string _address;


        private double _latitude;
        public double Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        private double _longitude;
        public double Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }



        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));
        public DelegateCommand TakePhotoCommand1 => _takePhotoCommand1 ?? (_takePhotoCommand1 = new DelegateCommand(TakePhoto1));
        public DelegateCommand TakePhotoCommand2 => _takePhotoCommand2 ?? (_takePhotoCommand2 = new DelegateCommand(TakePhoto2));
        public DelegateCommand TakePhotoCommand3 => _takePhotoCommand3 ?? (_takePhotoCommand3 = new DelegateCommand(TakePhoto3));
        public DelegateCommand GetAddressCommand => _getAddressCommand ?? (_getAddressCommand = new DelegateCommand(LoadSourceAsync));

        public DateTime Hoy { get; set; }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public int NroFoto;

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }


        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value);
        }

        public MediaFile File1
        {
            get => _file1;
            set => SetProperty(ref _file1, value);
        }
        public MediaFile File2
        {
            get => _file2;
            set => SetProperty(ref _file2, value);
        }
        public MediaFile File3
        {
            get => _file3;
            set => SetProperty(ref _file3, value);
        }


        public ImageSource ImageSource1
        {
            get => _imageSource1;
            set => SetProperty(ref _imageSource1, value);
        }

        public ImageSource ImageSource2
        {
            get => _imageSource2;
            set => SetProperty(ref _imageSource2, value);
        }
        public ImageSource ImageSource3
        {
            get => _imageSource3;
            set => SetProperty(ref _imageSource3, value);
        }


        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsRefreshing1
        {
            get => _isRefreshing1;
            set => SetProperty(ref _isRefreshing1, value);
        }

        public bool IsRefreshing2
        {
            get => _isRefreshing2;
            set => SetProperty(ref _isRefreshing2, value);
        }


   

        

        public AddLuminaryPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            Hoy = DateTime.Today;
            Date = DateTime.Today;
            IsEnabled = true;
            Title = "Agregar Nueva Luminaria";
            instance = this;
            ImageSource1 = "noimage.png";
            ImageSource2 = "noimage.png";
            ImageSource3 = "noimage.png";

            LoadTiposLuminaria();
            LoadEstados();

        }



        #region Singleton

        private static AddLuminaryPageViewModel instance;
        public static AddLuminaryPageViewModel GetInstance()
        {
            return instance;
        }
        #endregion

        private async void LoadSourceAsync()
        {
            IsEnabled = false;
            await _geolocatorService.GetLocationAsync();

            if (_geolocatorService.Latitude == 0 && _geolocatorService.Longitude == 0)
            {
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Error de Geolocalización",
                    "Aceptar");
                //await _navigationService.GoBackAsync();
                return;
            }

            _position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
            Latitude = _geolocatorService.Latitude;
            Longitude = _geolocatorService.Longitude;
            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(_position);
            List<string> addresses = new List<string>(sources);

            if (addresses.Count > 1)
            {
                Address = addresses[0];
            }

            IsEnabled = true;
        }


        private void LoadTiposLuminaria()
        {
            TiposLuminaria = new ObservableCollection<TipoLuminaria>();
            TiposLuminaria.Add(new TipoLuminaria { Codigo = 1, Descripcion = "Halógena", });
            TiposLuminaria.Add(new TipoLuminaria { Codigo = 2, Descripcion = "Led", });
            TiposLuminaria.Add(new TipoLuminaria { Codigo = 3, Descripcion = "Otra", });
        }

        private void LoadEstados()
        {
            Estados = new ObservableCollection<Estado>();
            Estados.Add(new Estado { Codigo = 1, Descripcion = "Pendiente", });
            Estados.Add(new Estado { Codigo = 2, Descripcion = "Funciona", });
            Estados.Add(new Estado { Codigo = 3, Descripcion = "Media Luz", });
            Estados.Add(new Estado { Codigo = 3, Descripcion = "No Funciona", });
        }

        private async void TakePhoto1()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "sourcePage", "Add" }
            };
            NroFoto = 1;
            await _navigationService.NavigateAsync("TakePhotoPage", parameters);
        }

        private async void TakePhoto2()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "sourcePage", "Add" }
            };
            NroFoto = 2;
            await _navigationService.NavigateAsync("TakePhotoPage", parameters);
        }

        private async void TakePhoto3()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "sourcePage", "Add" }
            };
            NroFoto = 3;
            await _navigationService.NavigateAsync("TakePhotoPage", parameters);
        }


        private async void Save()
            {
            if (TLuminaria == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar el Tipo de Luminaria.", "Aceptar");
                return;
            }

            if (string.IsNullOrEmpty(Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe ingresar la Dirección.", "Aceptar");
                return;
            }

            if (Latitude == 0 || Longitude == 0)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Se necesitan las Coordenadas de Geolocalización.", "Aceptar");
                return;
            }

            if (Estado == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe seleccionar el Estado de la Luminaria.", "Aceptar");
                return;
            }

            if (_file1 == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe cargar la foto de la Base de la Luminaria.", "Aceptar");
                return;
            }

            if (_file2 == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe cargar la foto de la Parte Superior de la Luminaria.", "Aceptar");
                return;
            }

            if (_file3 == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Debe cargar la foto de la Luminaria completa (desde lejos).", "Aceptar");
                return;
            }


            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = true;
                IsEnabled = false;
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Revise su conexión a Internet",
                    "Aceptar");
                return;
            }


            byte[] ImageArray1 = null;
            if (File1 != null)
            {
                ImageArray1 = FilesHelper.ReadFully(File1.GetStream());
                File1.Dispose();
            }

            byte[] ImageArray2 = null;
            if (File2 != null)
            {
                ImageArray2 = FilesHelper.ReadFully(File2.GetStream());
                File2.Dispose();
            }

            byte[] ImageArray3 = null;
            if (File3 != null)
            {
                ImageArray3 = FilesHelper.ReadFully(File3.GetStream());
                File3.Dispose();
            }

            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            LuminaryRequest myluminary = new LuminaryRequest
            {
                Address = Address,
                Date = DateTime.Today,
                Tipo = TLuminaria.Descripcion,
                Latitude = Latitude,
                Longitude = Longitude,
                Remarks = Remarks,
                BasePhotoArray = ImageArray1,
                TopPhotoArray = ImageArray2,
                FullPhotoArray = ImageArray3,
                State = Estado.Descripcion,
                UserId = user.Id,
                Neighborhood = user.Neighborhood.Name,
            };

            ResponseT<object> response = await _apiService.PostAsync(
            url,
            "api",
            "/Luminaries",
            myluminary,
            "bearer",
            token.Token);



            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;

                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    response.Message,
                    "Aceptar");
                return;
            }

            LuminaryRequest myLuminary = (LuminaryRequest)response.Result;

            IsRunning = false;
            IsEnabled = true;


            await App.Current.MainPage.DisplayAlert(
                "Ok",
                "Guardado con éxito!!",
                "Aceptar");

            //LuminariesPageViewModel luminariesPageViewModel = LuminariesPageViewModel.GetInstance();
            //luminariesPageViewModel.LoadLuminariesAsync();
            //luminariesPageViewModel.RefreshList();

            await _navigationService.GoBackAsync();
        }


        private async void Cancel()
        {
            await _navigationService.GoBackAsync();
        }
    }
}