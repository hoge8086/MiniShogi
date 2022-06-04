using System;
using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class CellViewModel<TKoma> : BindableBase where TKoma : class
    {
        public BoardPosition Position { get; set; }
        public ReactiveProperty<TKoma> Koma { get; set; }

        public CellViewModel()
        {
            Koma = new ReactiveProperty<TKoma>();
        }
        public virtual void ToEmpty()
        {
            Koma.Value = null;
        }
    }
}
