using MiniShogiMobile.Conditions;
using MiniShogiMobile.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.GameTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniShogiMobile.ViewModels
{
    public class EnumWinConditionTypeProvider : EnumListProvider<WinConditionType> { }
    public class EditGameSettingsPopupPageViewModel : NavigationViewModel<GameTemplate, GameTemplate>
    {
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand CancelCommand { get; }
        public ReactiveProperty<int> Height { get; }
        public ReactiveProperty<int> Width { get; }
        public ReactiveProperty<int> TerritoryBoundary { get; }
        public ReactiveProperty<WinConditionType> WinCondition { get; }
        public ReactiveProperty<bool> EnableNiHu { get; }
        public ReactiveProperty<bool> EnableCheckmateByHandHu { get; }
        public ReactiveProperty<bool> EnableKomaCannotMove { get; }
        public ReactiveProperty<bool> EnableLeaveOte { get; }
        GameTemplate GameTemplate;

        public EditGameSettingsPopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Height = new ReactiveProperty<int>();
            Width = new ReactiveProperty<int>();
            TerritoryBoundary = new ReactiveProperty<int>();
            WinCondition = new ReactiveProperty<WinConditionType>();
            EnableNiHu = new ReactiveProperty<bool>();
            EnableCheckmateByHandHu = new ReactiveProperty<bool>();
            EnableKomaCannotMove = new ReactiveProperty<bool>();
            EnableLeaveOte = new ReactiveProperty<bool>();
            
            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                GameTemplate.Height = Height.Value;
                GameTemplate.Width = Width.Value;
                GameTemplate.TerritoryBoundary = TerritoryBoundary.Value;
                GameTemplate.WinCondition = WinCondition.Value;
                GameTemplate.ProhibitedMoves = new ProhibitedMoves(
                    EnableNiHu.Value,
                    EnableCheckmateByHandHu.Value,
                    EnableKomaCannotMove.Value,
                    EnableLeaveOte.Value
                    );
                await GoBackAsync(GameTemplate);
            }).AddTo(this.Disposable);

            CancelCommand = new AsyncReactiveCommand();
            CancelCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
        }

        public override void Prepare(GameTemplate parameter)
        {
            GameTemplate = parameter.Clone();
            Height.Value = parameter.Height;
            Width.Value = parameter.Width;
            TerritoryBoundary.Value = parameter.TerritoryBoundary;
            WinCondition.Value = parameter.WinCondition;
            EnableNiHu.Value = parameter.ProhibitedMoves.EnableNiHu;
            EnableKomaCannotMove.Value = parameter.ProhibitedMoves.EnableKomaCannotMove;
            EnableCheckmateByHandHu.Value = parameter.ProhibitedMoves.EnableCheckmateByHandHu;
            EnableLeaveOte.Value = parameter.ProhibitedMoves.EnableLeaveOte;
        }
    }
}
