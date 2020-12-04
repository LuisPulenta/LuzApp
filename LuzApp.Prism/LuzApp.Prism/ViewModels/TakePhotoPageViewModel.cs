using LuzApp.Common.Services;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;


namespace LuzApp.Prism.ViewModels
{
    public class TakePhotoPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;




        private bool _isRunning;
        private bool _isEnabled;
        private string _sourcePage;

        private ImageSource _imageSource;
        private MediaFile _file;


        public string SourcePage
        {
            get => _sourcePage;
            set => SetProperty(ref _sourcePage, value);
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

        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        private DelegateCommand _cancelCommand;
        private DelegateCommand _saveCommand;
        private DelegateCommand _takePhotoCommand;

        public DelegateCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new DelegateCommand(Cancel));
        public DelegateCommand SaveCommand => _saveCommand ?? (_saveCommand = new DelegateCommand(Save));
        public DelegateCommand TakePhotoCommand => _takePhotoCommand ?? (_takePhotoCommand = new DelegateCommand(TakePhoto));

        public TakePhotoPageViewModel(INavigationService navigationService, IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = "Tomar Fotografía";
            IsEnabled = true;
            ImageSource = "noimage";
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            SourcePage = parameters.GetValue<string>("sourcePage");
        }


        private async void Cancel()
        {
            await _navigationService.GoBackAsync();
        }



        private async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();





            _file = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    Directory = "Sample",
                    Name = "test.jpg",
                    PhotoSize = PhotoSize.Small,
                }
            );


            if (_file != null)
            {
                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = _file.GetStream();
                    return stream;
                });




            }
            IsRunning = false;
        }


        private async void Save()
        {

            if (SourcePage == "Add")
            {
                var addLuminaryPageViewModel = AddLuminaryPageViewModel.GetInstance();

                if (addLuminaryPageViewModel.NroFoto == 1)
                {

                    if (_file == null)
                    {
                        addLuminaryPageViewModel.ImageSource1 = "noimage";
                    }
                    else
                    {
                        addLuminaryPageViewModel.ImageSource1 = ImageSource.FromStream(() =>
                        {
                            var stream = _file.GetStream();
                            return stream;
                        });
                        addLuminaryPageViewModel.File1 = _file;
                    }
                }

                if (addLuminaryPageViewModel.NroFoto == 2)
                {

                    if (_file == null)
                    {
                        addLuminaryPageViewModel.ImageSource2 = "noimage";
                    }
                    else
                    {
                        addLuminaryPageViewModel.ImageSource2 = ImageSource.FromStream(() =>
                        {
                            var stream = _file.GetStream();
                            return stream;
                        });
                        addLuminaryPageViewModel.File2 = _file;
                    }
                }
                if (addLuminaryPageViewModel.NroFoto == 3)
                {

                    if (_file == null)
                    {
                        addLuminaryPageViewModel.ImageSource3 = "noimage";
                    }
                    else
                    {
                        addLuminaryPageViewModel.ImageSource3 = ImageSource.FromStream(() =>
                        {
                            var stream = _file.GetStream();
                            return stream;
                        });
                        addLuminaryPageViewModel.File3 = _file;
                    }
                }



                await _navigationService.GoBackAsync();
            }


            //if (SourcePage == "Edit")
            //{
            //    var editRecipePageViewModel = EditItemPageViewModel.GetInstance();

            //    if (editRecipePageViewModel.NroFoto == 1)
            //    {

            //        if (_file == null)
            //        {
            //            editRecipePageViewModel.ImageSource1 = "noimage";
            //        }
            //        else
            //        {
            //            editRecipePageViewModel.ImageSource1 = ImageSource.FromStream(() =>
            //            {
            //                var stream = _file.GetStream();
            //                return stream;
            //            });
            //            editRecipePageViewModel.File1 = _file;
            //        }
            //    }

            //    if (editRecipePageViewModel.NroFoto == 2)
            //    {

            //        if (_file == null)
            //        {
            //            editRecipePageViewModel.ImageSource2 = "noimage";
            //        }
            //        else
            //        {
            //            editRecipePageViewModel.ImageSource2 = ImageSource.FromStream(() =>
            //            {
            //                var stream = _file.GetStream();
            //                return stream;
            //            });
            //            editRecipePageViewModel.File2 = _file;
            //        }
            //    }




            await _navigationService.GoBackAsync();
        }

    }
}
