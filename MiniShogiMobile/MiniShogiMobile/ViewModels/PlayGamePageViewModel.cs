using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class PlayGamePageViewModel : ViewModelBase
    {
        public PlayGamePageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var val = parameters[nameof(PlayGameCondition)] as PlayGameCondition;
            if(val == null)
            {
                throw new ArgumentException(nameof(PlayGameCondition));
            }

            Title = val.Name;
        }
    }
}
