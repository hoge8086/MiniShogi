using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class CellBaseViewModel : BindableBase
    {
        public BoardPosition Position { get; set; }
        public ReactiveProperty<KomaViewModel> Koma { get; set; }

        public CellBaseViewModel()
        {
            Koma = new ReactiveProperty<KomaViewModel>();
        }
    }
    public class CellPlayingViewModel : CellBaseViewModel, ISelectable
    {
        public ReactiveProperty<List<MoveCommand>> MoveCommands { get; set; }

        public ReadOnlyReactivePropertySlim<bool> CanMove { get; set; }

        public ReactiveProperty<bool> IsSelected { get; set; }
        public CellPlayingViewModel()
        {
            MoveCommands = new ReactiveProperty<List<MoveCommand>>();
            //CanMove = new ReactiveProperty<bool>();
            IsSelected = new ReactiveProperty<bool>(false);
            //MoveCommands.Subscribe((x) => { CanMove.Value == null; });
            CanMove = MoveCommands.Select((x) => x != null).ToReadOnlyReactivePropertySlim();
        }
        public void Reset()
        {
            Koma.Value = null;
            IsSelected.Value = false;
            MoveCommands.Value = null;

        }
        public void Select()
        {
            IsSelected.Value = true;
        }
    }

}
