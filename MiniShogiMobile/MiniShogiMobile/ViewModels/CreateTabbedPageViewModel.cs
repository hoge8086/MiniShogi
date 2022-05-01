using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class CreateTabbedPageViewModel : NavigationViewModel
    {
        public CreateTabbedPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

        }
    }
}
