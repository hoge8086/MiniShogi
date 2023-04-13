using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;

namespace MiniShogiMobile.ViewModels
{
    public static class NavigationViewModelEx
    {
        public static async Task CatchErrorWithMessageAsync(this NavigationViewModel vm, Func<Task> action)
        {
            try
            {
                await action();
            }
            catch(Exception ex)
            {
                await vm.PageDialogService.DisplayAlertAsync("エラー(不具合)", ex.Message, "OK");
            }
        }
    }
}
