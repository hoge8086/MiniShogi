using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MiniShogiMobile.ViewModels
{
	public class MessagePopupPageViewModel : NavigationViewModel<string, object>
	{
        public ReactiveProperty<string> Message { get; set; }
        public MessagePopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Message = new ReactiveProperty<string>();
        }
        public override void Prepare(string messega)
        {
            Message.Value = messega;
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(500);
                // 何かを返さないと呼び出し元で表示の終了を待たないため、objectを返す
                await GoBackAsync(new object());
            });
        }
	}
}
