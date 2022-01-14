using MiniShogiMobile.Conditions;
using MiniShogiMobile.Utils;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.GameTemplates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class EnumWinConditionTypeProvider : EnumListProvider<WinConditionType> { }
    public class WinConditionTypeConverter : EnumToDescriptionConverter<WinConditionType> { }
    public class EditGameSettingsPageViewModel : ViewModelBase
    {
        public ReactiveCommand OkCommand { get; }
        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<int> Height { get; }
        public ReactiveProperty<int> Width { get; }
        public ReactiveProperty<int> TerritoryBoundary { get; }
        public ReactiveProperty<WinConditionType> WinCondition { get; }
        public ReactiveProperty<bool> EnableNiHu { get; }
        public ReactiveProperty<bool> EnableCheckmateByHandHu { get; }
        public ReactiveProperty<bool> EnableKomaCannotMove { get; }
        public ReactiveProperty<bool> EnableLeaveOte { get; }
        private GameTemplate GameTemplate;

        public EditGameSettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Name = new ReactiveProperty<string>();
            Height = new ReactiveProperty<int>();
            Width = new ReactiveProperty<int>();
            TerritoryBoundary = new ReactiveProperty<int>();
            WinCondition = new ReactiveProperty<WinConditionType>();
            EnableNiHu = new ReactiveProperty<bool>();
            EnableCheckmateByHandHu = new ReactiveProperty<bool>();
            EnableKomaCannotMove = new ReactiveProperty<bool>();
            EnableLeaveOte = new ReactiveProperty<bool>();
            
            OkCommand = new ReactiveCommand();
            OkCommand.Subscribe(() =>
            {
                GameTemplate.Name = Name.Value;
                GameTemplate.Height = Height.Value;
                GameTemplate.Width = Width.Value;
                GameTemplate.TerritoryBoundary = TerritoryBoundary.Value;
                GameTemplate.WinCondition = WinCondition.Value;
                GameTemplate.ProhibitedMoves.EnableNiHu = EnableNiHu.Value;
                GameTemplate.ProhibitedMoves.EnableKomaCannotMove = EnableKomaCannotMove.Value;
                GameTemplate.ProhibitedMoves.EnableCheckmateByHandHu = EnableCheckmateByHandHu.Value;
                GameTemplate.ProhibitedMoves.EnableLeaveOte = EnableLeaveOte.Value;
                navigationService.GoBackAsync();
            }).AddTo(this.Disposable);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(EditDetailGameSettingsCondition)] as EditDetailGameSettingsCondition;
            if(param == null)
                throw new ArgumentException(nameof(EditDetailGameSettingsCondition));

            GameTemplate = param.GameTemplate;
            Name.Value = GameTemplate.Name;
            Height.Value = GameTemplate.Height;
            Width.Value = GameTemplate.Width;
            TerritoryBoundary.Value = GameTemplate.TerritoryBoundary;
            WinCondition.Value = GameTemplate.WinCondition;
            EnableNiHu.Value = GameTemplate.ProhibitedMoves.EnableNiHu;
            EnableKomaCannotMove.Value = GameTemplate.ProhibitedMoves.EnableKomaCannotMove;
            EnableCheckmateByHandHu.Value = GameTemplate.ProhibitedMoves.EnableCheckmateByHandHu;
            EnableLeaveOte.Value = GameTemplate.ProhibitedMoves.EnableLeaveOte;
        }
    }
}
