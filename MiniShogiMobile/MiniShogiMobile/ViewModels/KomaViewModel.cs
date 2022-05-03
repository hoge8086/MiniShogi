using Reactive.Bindings;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Reactive.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class KomaViewModel
    {
        public ReactiveProperty<bool> IsTransformed { get; private set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<PlayerType> PlayerType { get; private set; } = new ReactiveProperty<PlayerType>();
        public ReadOnlyReactiveProperty<string> Name { get; private set; }
        public ReactiveProperty<KomaTypeId> KomaTypeId = new ReactiveProperty<KomaTypeId>();

        private ReadOnlyReactiveProperty<string> CreateNameReactiveProperty()
        {
            return IsTransformed.CombineLatest(KomaTypeId, Tuple.Create)
                                     .Select(x  => IsTransformed.Value  ? this.KomaTypeId.Value.PromotedName : this.KomaTypeId.Value.Name)
                                     .ToReadOnlyReactiveProperty(); 
                                     // Fix: AddTo(Disposableをどうする?ここは不要?)
        }
        public KomaViewModel(Koma koma)
        {
            this.KomaTypeId.Value = koma.TypeId;
            this.IsTransformed.Value = koma.IsTransformed;
            this.PlayerType.Value = koma.Player;
            this.Name = CreateNameReactiveProperty();
        }
        public KomaViewModel(KomaTypeId komaTypeId, PlayerType playerType, bool isTransformed)
        {
            this.KomaTypeId.Value = komaTypeId;
            this.IsTransformed.Value = isTransformed;
            this.PlayerType.Value = playerType;
            this.Name = CreateNameReactiveProperty();
        }
        public KomaViewModel(KomaViewModel other)
        {
            this.KomaTypeId.Value = other.KomaTypeId.Value;
            this.IsTransformed.Value = other.IsTransformed.Value;
            this.PlayerType.Value = other.PlayerType.Value;
            this.Name = CreateNameReactiveProperty();
        }
        public KomaViewModel()
        {
            this.KomaTypeId.Value = new KomaTypeId();
            this.IsTransformed.Value = false;
            this.PlayerType.Value = Shogi.Business.Domain.Model.PlayerTypes.PlayerType.Player1;
            this.Name = CreateNameReactiveProperty();
        }


        public void Update(KomaViewModel other)
        {
            if (other == null)
                return;
            this.KomaTypeId.Value = other.KomaTypeId.Value;
            this.IsTransformed.Value = other.IsTransformed.Value;
            this.PlayerType.Value = other.PlayerType.Value;
        }
    }
}
