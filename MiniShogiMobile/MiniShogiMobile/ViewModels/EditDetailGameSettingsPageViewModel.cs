using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.GameTemplates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class EditDetailGameSettingsPageViewModel : BindableBase
    {
        public ReactiveProperty<string> Name { get; }

        public ReactiveProperty<int> Height { get; }
        public ReactiveProperty<int> Width { get; }
        public ReactiveProperty<int> TerritoryBoundary { get; }
        public ReactiveProperty<WinConditionType> WinConditionType { get; }
        public ReactiveProperty<bool> EnableNiHu { get; }
        public ReactiveProperty<bool> EnableCheckmateByHandHu { get; }
        public ReactiveProperty<bool> EnableKomaCannotMove { get; }
        public ReactiveProperty<bool> EnableLeaveOte { get; }

        public EditDetailGameSettingsPageViewModel()
        {

        }
    }
}
