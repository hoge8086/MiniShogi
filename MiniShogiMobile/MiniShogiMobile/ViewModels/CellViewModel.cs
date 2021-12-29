using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System.Collections.Generic;
using System.Reactive.Linq;

using System.Reactive.Disposables;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System;

namespace MiniShogiMobile.ViewModels
{

    public class KomaViewModel
    {
        public bool IsTransformed { get; set; }
        public PlayerType PlayerType { get; set; }

        public string Name { get; set; }
    }
    public class CellViewModel : BindableBase
    {
        public BoardPosition Position { get; set; }
        public KomaViewModel Koma { get; set; }
        public string KomaName => Koma == null ? "" : Koma.Name;

        public ReactiveProperty<List<MoveCommand>> MoveCommands {get; set;}

        public ReactiveProperty<bool> CanMove { get; set; }

        public CellViewModel()
        {
            MoveCommands = new ReactiveProperty<List<MoveCommand>>();
            CanMove = new ReactiveProperty<bool>();
            //MoveCommands.Subscribe((x) => { CanMove.Value = true; });

            var a = new ReactiveProperty<string>();
            a.Subscribe((x) => { });
        }

    }
}
