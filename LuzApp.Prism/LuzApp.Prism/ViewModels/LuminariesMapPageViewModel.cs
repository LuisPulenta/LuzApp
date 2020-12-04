using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuzApp.Prism.ViewModels
{
    public class LuminariesMapPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        public LuminariesMapPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Mapa de Luminarias";
        }
    }
}