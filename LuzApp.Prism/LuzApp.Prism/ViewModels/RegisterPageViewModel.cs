using LuzApp.Common.Entities;
using LuzApp.Common.Helpers;
using LuzApp.Common.Requests;
using LuzApp.Common.Responses;
using LuzApp.Common.Services;
using LuzApp.Prism.Helpers;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LuzApp.Prism.ViewModels
{
    public class RegisterPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IRegexHelper _regexHelper;
        private readonly IApiService _apiService;
        private readonly IGeolocatorService _geolocatorService;
        private readonly IFilesHelper _filesHelper;
        private ImageSource _image;
        private UserRequest _user;
        private Neighborhood _neighborhood;
        private ObservableCollection<Neighborhood> _neighborhoods;
        private City _city;
        private ObservableCollection<City> _cities;
        private Department _department;
        private ObservableCollection<Department> _departments;
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _registerCommand;

        private MediaFile _file;
        private DelegateCommand _changeImageCommand;
        public DelegateCommand ChangeImageCommand => _changeImageCommand ?? (_changeImageCommand = new DelegateCommand(ChangeImageAsync));


        public RegisterPageViewModel(
            INavigationService navigationService,
            IRegexHelper regexHelper,
            IApiService apiService,
            IGeolocatorService geolocatorService,
            IFilesHelper filesHelper)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _regexHelper = regexHelper;
            _apiService = apiService;
            _geolocatorService = geolocatorService;
            _filesHelper = filesHelper;
            Title = "Registrar Nuevo Usuario";
            Image = App.Current.Resources["UrlNoImage"].ToString();
            IsEnabled = true;
            User = new UserRequest();
            LoadDepartmentsAsync();

        }

        public DelegateCommand RegisterCommand => _registerCommand ?? (_registerCommand = new DelegateCommand(RegisterAsync));

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public UserRequest User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public Department Department
        {
            get => _department;
            set
            {
                Cities = value != null ? new ObservableCollection<City>(value.Cities) : null;
                Neighborhoods = new ObservableCollection<Neighborhood>();
                City = null;
                Neighborhood = null;
                SetProperty(ref _department, value);
            }
        }

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set => SetProperty(ref _departments, value);
        }

        public City City
        {
            get => _city;
            set
            {
                Neighborhoods = value != null ? new ObservableCollection<Neighborhood>(value.Neighborhoods) : null;
                Neighborhood = null;
                SetProperty(ref _city, value);
            }
        }

        public ObservableCollection<City> Cities
        {
            get => _cities;
            set => SetProperty(ref _cities, value);
        }

        public Neighborhood Neighborhood
        {
            get => _neighborhood;
            set => SetProperty(ref _neighborhood, value);
        }

        public ObservableCollection<Neighborhood> Neighborhoods
        {
            get => _neighborhoods;
            set => SetProperty(ref _neighborhoods, value);
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

        private async void LoadDepartmentsAsync()
        {
            IsRunning = true;
            IsEnabled = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "Error de Conexión", "Aceptar");
                return;
            }

            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.GetListAsync<Department>(url, "api", "/Departments");
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                return;
            }

            List<Department> list = (List<Department>)response.Result;
            Departments = new ObservableCollection<Department>(list.OrderBy(c => c.Name));
        }

        private async void RegisterAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert("Error", "ConnectionError", "Aceptar");
                return;
            }

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

           
            User.ImageArray = imageArray;

            string url = App.Current.Resources["UrlAPI"].ToString();

            User.NeighborhoodId = Neighborhood.Id;

            Response response = await _apiService.RegisterUserAsync(url, "api", "/Account/Register", User);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                if (response.Message == "Error003")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Este usuario ya existe", "Aceptar");
                }
                else if (response.Message == "Error004")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La ciudad no es válida", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                }

                return;
            }
            await App.Current.MainPage.DisplayAlert("Ok", "Se le ha enviado un mail para confirmar el Registro. Por favor chequee su correo.", "Aceptar");
            await _navigationService.GoBackAsync();
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(User.Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese Documento", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.FirstName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Nombre", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.LastName))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Apellido", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Address))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese una Dirección", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Email) || !_regexHelper.IsValidEmail(User.Email))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un EMail", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Phone))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Teléfono", "Aceptar");
                return false;
            }

            if (Department == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un país", "Aceptar");
                return false;
            }

            if (City == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione una Ciudad", "Aceptar");
                return false;
            }

            if (Neighborhood == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un Barrio", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.Password) || User.Password?.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Password", "Aceptar");
                return false;
            }

            if (string.IsNullOrEmpty(User.PasswordConfirm))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese una Conf. de Password", "Aceptar");
                return false;
            }

            if (User.Password != User.PasswordConfirm)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Password y su Confirmación deben ser iguales", "Accept");
                return false;
            }
            return true;
        }

        private async void ChangeImageAsync()
        {
            await CrossMedia.Current.Initialize();

            string source = await Application.Current.MainPage.DisplayActionSheet(
                "De donde quiere tomar la foto?",
                "Cancelar",
                null,
                "Galería",
                "Cámara");

            if (source == "Cancelar")
            {
                _file = null;
                return;
            }

            if (source == "Cámara")
            {
                if (!CrossMedia.Current.IsCameraAvailable)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La cámara no está disponible", "Aceptar");
                    return;
                }

                _file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La Galería no está disponible", "Aceptar");
                    return;
                }

                _file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (_file != null)
            {
                Image = ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }


    }
}