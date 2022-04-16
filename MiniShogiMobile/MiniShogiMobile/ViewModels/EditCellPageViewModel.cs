using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Komas;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;

namespace MiniShogiMobile.ViewModels
{
    public class EditCellPageViewModel : ViewModelBase
    {
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public AsyncReactiveCommand DeleteCommand { get; }
        public CellViewModel Cell { get; private set; }
        public ObservableCollection<string> KomaNameList { get; }
        public Dictionary<string, KomaType> KomaTypes { get; }

        public CellViewModel EditingCell { get; private set; }
        public ReactiveProperty<bool> CanTransform { get; private set; }

        public ReactiveProperty<bool>ShowDeleteKomaCommand { get; }

        public EditCellPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaTypes =  App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            KomaNameList = new ObservableCollection<string>(KomaTypes.Keys);
            CanTransform = new ReactiveProperty<bool>(false);
            ShowDeleteKomaCommand = new ReactiveProperty<bool>(false);
            EditingCell = new CellViewModel() { Koma = new ReactiveProperty<KomaViewModel>() };
            EditingCell.Koma.Value = new KomaViewModel(KomaTypes.First().Value.Id, PlayerType.Player1, false);
            EditingCell.Koma.Value.Name.Subscribe(x =>
            {
                CanTransform.Value = KomaTypes[x].CanBeTransformed;
                EditingCell.Koma.Value.IsTransformed.Value &= KomaTypes[x].CanBeTransformed;
            }).AddTo(this.Disposable);


            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                Cell.Koma.Value = new KomaViewModel(EditingCell.Koma.Value);
                await navigationService.GoBackAsync();
            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await navigationService.GoBackAsync();
            });
            DeleteCommand = new AsyncReactiveCommand();
            DeleteCommand.Subscribe(async () =>
            {
                // 確認メッセージはうっとうしい可能性あり
                //bool doDelete = await pageDialogService.DisplayAlertAsync("確認", "削除しますか?", "はい", "いいえ");
                //if (doDelete)
                //{
                    Cell.Koma.Value = null;
                    await navigationService.GoBackAsync();
                //}
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(EditCellCondition)] as EditCellCondition;
            if(param == null)
                throw new ArgumentException(nameof(EditCellCondition));

            ShowDeleteKomaCommand.Value = param.ShowDeleteKomaCommand;
            Cell = param.Cell;
            if (Cell.Koma.Value != null)
                EditingCell.Koma.Value.Update(Cell.Koma.Value);
            else
                EditingCell.Koma.Value.PlayerType.Value = ((param.Height / 2) > Cell.Position.Y) ? PlayerType.Player2 : PlayerType.Player1;
        }
    }
}
