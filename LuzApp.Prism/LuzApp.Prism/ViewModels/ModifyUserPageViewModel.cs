using FFImageLoading.Work;
using Newtonsoft.Json;
using LuzApp.Common.Entities;
using LuzApp.Common.Helpers;
using LuzApp.Common.Requests;
using LuzApp.Common.Responses;
using LuzApp.Common.Services;
using LuzApp.Prism.Views;
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
using ImageSource = Xamarin.Forms.ImageSource;

namespace LuzApp.Prism.ViewModels
{
    public class ModifyUserPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IFilesHelper _filesHelper;
        private ImageSource _image;
        private UserResponse _user;
        private Neighborhood _neighborhood;
        private ObservableCollection<Neighborhood> _neighborhoods;
        private City _city;
        private ObservableCollection<City> _cities;
        private Department _department;
        private ObservableCollection<Department> _departments;
        private bool _isRunning;
        private bool _isEnabled;
        private MediaFile _file;
        private DelegateCommand _changeImageCommand;
        private DelegateCommand _saveCommand;
        private DelegateCommand _changePasswordCommand;

        public ModifyUserPageViewModel(
            INavigationService navigationService,
            IApiService apiService,
            IFilesHelper filesHelper)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _filesHelper = filesHelper;
            Title = "Modificar Usuario";
            IsEnabled = true;
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            User = token.User;
            Image = User.ImageFullPath;
            LoadDepartmentsAsync();
        }

        public DelegateCommand ChangeImageCommand => _changeImageCommand ??
            (_changeImageCommand = new DelegateCommand(ChangeImageAsync));

        public DelegateCommand SaveCommand => _saveCommand ??
            (_saveCommand = new DelegateCommand(SaveAsync));

        public DelegateCommand ChangePasswordCommand => _changePasswordCommand ??
            (_changePasswordCommand = new DelegateCommand(ChangePasswordAsync));

        public Xamarin.Forms.ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public UserResponse User
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
            get
            {
                return _neighborhood;
            }

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
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "No hay conexión a Internet",
                    "Accept");
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
            LoadCurrentProvinciasCiudadesBarrios();
        }

        private void LoadCurrentProvinciasCiudadesBarrios()
        {
            Department = Departments.FirstOrDefault(c => c.Cities.FirstOrDefault(d => d.Neighborhoods.FirstOrDefault(ci => ci.Id == User.Neighborhood.Id) != null) != null);
            City = Department.Cities.FirstOrDefault(d => d.Neighborhoods.FirstOrDefault(c => c.Id == User.Neighborhood.Id) != null);
            Neighborhood = City.Neighborhoods.FirstOrDefault(c => c.Id == User.Neighborhood.Id);
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
                Image = Xamarin.Forms.ImageSource.FromStream(() =>
                {
                    System.IO.Stream stream = _file.GetStream();
                    return stream;
                });
            }
        }

        private async void SaveAsync()
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
                await App.Current.MainPage.DisplayAlert("Error", "No hay conexión a Internet", "Aceptar");
                return;
            }

            byte[] imageArray = null;
            if (_file != null)
            {
                imageArray = _filesHelper.ReadFully(_file.GetStream());
            }

            UserRequest request = new UserRequest
            {
                Address = User.Address,
                NeighborhoodId = Neighborhood.Id,
                Document = User.Document,
                Email = User.Email,
                FirstName = User.FirstName,
                ImageArray = imageArray,
                LastName = User.LastName,
                Password = "123456", // Doen't matter, it's only to pass the data annotation
                Phone = User.PhoneNumber,
            };

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.ModifyUserAsync(url, "api", "/Account", request, token.Token);
            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                if (response.Message == "Error001")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "El Usuario no existe", "Aceptar");
                }
                else if (response.Message == "Error004")
                {
                    await App.Current.MainPage.DisplayAlert("Error", "La ciudad no existe", "Aceptar");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", response.Message, "Aceptar");
                }

                return;
            }

            UserResponse updatedUser = (UserResponse)response.Result;
            token.User = updatedUser;
            Settings.Token = JsonConvert.SerializeObject(token);
            LuzAppMasterDetailPageViewModel.GetInstance().LoadUser();
            await App.Current.MainPage.DisplayAlert("Ok", "Se guardaron los cambios con éxito!!", "Aceptar");
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(User.Document))
            {
                await App.Current.MainPage.DisplayAlert("Error", "Ingrese un Documento", "Aceptar");
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

            if (string.IsNullOrEmpty(User.PhoneNumber))
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
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un departamento", "Aceptar");
                return false;
            }

            if (Neighborhood == null)
            {
                await App.Current.MainPage.DisplayAlert("Error", "Seleccione un Barrio", "Aceptar");
                return false;
            }

            return true;
        }

        private async void ChangePasswordAsync()
        {
            await _navigationService.NavigateAsync(nameof(ChangePasswordPage));
        }
    }

}