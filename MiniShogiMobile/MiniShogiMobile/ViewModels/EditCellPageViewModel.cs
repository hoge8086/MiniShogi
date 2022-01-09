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
        public ReactiveCommand OkCommand { get; }
        public ReactiveCommand ChangePlayerCommand { get; }

        public CellViewModel Cell { get; private set; }

        public ObservableCollection<string> KomaNameList { get; }
        public Dictionary<string, KomaType> KomaTypes { get; }

        public CellViewModel EditingCell { get; private set; }
        //public KomaViewModel EditingKoma { get; private set; }
        public ReactiveProperty<bool> HasKoma { get; private set; }
        public ReactiveProperty<bool> CanTransform { get; private set; }

        public EditCellPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            HasKoma = new ReactiveProperty<bool>();
            KomaTypes =  App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            KomaNameList = new ObservableCollection<string>(KomaTypes.Keys);
            CanTransform = new ReactiveProperty<bool>(false);
            EditingCell = new CellViewModel() { Koma = new ReactiveProperty<KomaViewModel>() };
            EditingCell.Koma.Value = new KomaViewModel(KomaTypes.First().Value.Id, PlayerType.Player1, false);
            EditingCell.Koma.Value.Name.Subscribe(x =>
            {
                CanTransform.Value = KomaTypes[x].CanBeTransformed;
                EditingCell.Koma.Value.IsTransformed.Value &= KomaTypes[x].CanBeTransformed;
            }).AddTo(this.Disposable);

            ChangePlayerCommand = new ReactiveCommand();
            ChangePlayerCommand.Subscribe(() => EditingCell.Koma.Value.PlayerType.Value = EditingCell.Koma.Value.PlayerType.Value.Opponent);

            OkCommand = new ReactiveCommand();
            OkCommand.Subscribe(() =>
            {
                Cell.Koma.Value = HasKoma.Value ? new KomaViewModel(EditingCell.Koma.Value) : null;
                this.Disposable.Dispose();
                navigationService.GoBackAsync();
            });
        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(EditCellCondition)] as EditCellCondition;
            if(param == null)
                throw new ArgumentException(nameof(EditCellCondition));

            Cell = param.Cell;
            EditingCell.Koma.Value.Update(Cell.Koma.Value);
            // [駒がないセルを押下した場合は、駒を追加したいからであるため、デフォはONの方が使いやすい]
            HasKoma.Value = true;// Cell.Koma.Value != null;
        }
    }
}
