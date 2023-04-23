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
	public class LicensePageViewModel : NavigationViewModel
	{
        public LicensePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

        }
	}
}
