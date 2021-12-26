using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reactive.Bindings;

namespace MiniShogiMobile.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Label { get; set; }
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Label = new ReactiveProperty<string>("あいうえお");
            Title = "Main Page";
        }
    }
}
