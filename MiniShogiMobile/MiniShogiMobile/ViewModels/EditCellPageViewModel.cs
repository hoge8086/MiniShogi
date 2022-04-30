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
using MiniShogiMobile.Views;
using Prism.NavigationEx;
using System.Threading.Tasks;

namespace MiniShogiMobile.ViewModels
{
    public class EditCellPageViewModel : NavigationViewModel<KomaViewModel, KomaViewModel>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public AsyncReactiveCommand<object> ChangeKomaTypeCommand { get; }
        private Dictionary<string, KomaType> KomaTypes { get; }

        public CellViewModel EditingCell { get; private set; }
        public ReactiveProperty<bool> CanTransform { get; private set; }

        public EditCellPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            KomaTypes =  App.CreateGameService.KomaTypeRepository.FindAll().ToDictionary(x => x.Id);
            CanTransform = new ReactiveProperty<bool>(false);
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
                await GoBackAsync(EditingCell.Koma.Value);
            }).AddTo(this.Disposable);
            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
            ChangeKomaTypeCommand = new AsyncReactiveCommand<object>();
            ChangeKomaTypeCommand.Subscribe(async (x) =>
            {
                var condition = new SelectKomaConditions(EditingCell.Koma.Value.Name.Value);
                var result = await NavigateAsync<SelectKomaPageViewModel, SelectKomaConditions, string>(condition);
                if (result.Success)
                {
                    EditingCell.Koma.Value.Name.Value = result.Data;
                }
            }).AddTo(this.Disposable);
        }
        public override void Prepare(KomaViewModel parameter)
        {
            EditingCell.Koma.Value.Update(parameter);

        }
    }
}
