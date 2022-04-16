using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class SelectKomaPageViewModel : ViewModelBase
    {
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public ObservableCollection<string> KomaNameList { get; }
        public ReactiveProperty<string> SelectedKomaName { get; }

        public SelectKomaPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaNameList = new ObservableCollection<string>();
            SelectedKomaName = new ReactiveProperty<string>();

            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                var param = new NavigationParameters();
                param.Add(nameof(SelectKomaConditions), new SelectKomaConditions(null, SelectedKomaName.Value)); ;
                await navigationService.GoBackAsync(param);

            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await navigationService.GoBackAsync();
            }).AddTo(this.Disposable);
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(SelectKomaConditions)] as SelectKomaConditions;
            if (param == null)
                throw new ArgumentException(nameof(SelectKomaConditions));

            foreach(var name in param.KomaNameList)
                KomaNameList.Add(name);
            if (param.SelectedKoma == null)
                SelectedKomaName.Value = KomaNameList.FirstOrDefault();
            else
                SelectedKomaName.Value = param.SelectedKoma;
        }
    }
}
