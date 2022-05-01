using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;

namespace MiniShogiMobile.ViewModels
{
    public class CreateKomaListPageViewModel : NavigationViewModel
    {
        public CreateKomaListPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

        }
    }
}
