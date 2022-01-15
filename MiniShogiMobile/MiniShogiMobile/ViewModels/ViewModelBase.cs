using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;

namespace MiniShogiMobile.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware, IDestructible
    {
        protected CompositeDisposable Disposable { get; } = new CompositeDisposable();
        public INavigationService NavigationService { get; private set; }
        public IPageDialogService PageDialogService { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            NavigationService = navigationService;
            PageDialogService = pageDialogService;
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public async virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {
            this.Disposable.Dispose();
        }

        public void CatchErrorWithMessage(Action action)
        {
            try
            {
                action();
            }
            catch(Exception ex)
            {
                PageDialogService.DisplayAlertAsync("エラー", ex.Message, "OK");
            }
        }
    }
}
