using Reactive.Bindings;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;

namespace MiniShogiMobile.ViewModels
{
    public class KomaViewModel
    {
        public KomaViewModel(string name, PlayerType playerType, bool isTransformed)
        {
            this.Name.Value = name;
            this.IsTransformed.Value = isTransformed;
            this.PlayerType.Value = playerType;
        }
        public KomaViewModel(KomaViewModel other)
        {
            this.Name.Value = other.Name.Value;
            this.IsTransformed.Value = other.IsTransformed.Value;
            this.PlayerType.Value = other.PlayerType.Value;
        }
        public KomaViewModel()
        {
            this.Name.Value = "";
            this.IsTransformed.Value = false;
            this.PlayerType.Value = Shogi.Business.Domain.Model.PlayerTypes.PlayerType.Player1;
        }

        public ReactiveProperty<bool> IsTransformed { get; set; } = new ReactiveProperty<bool>();
        public ReactiveProperty<PlayerType> PlayerType { get; set; } = new ReactiveProperty<PlayerType>();
        public ReactiveProperty<string> Name { get; set; } = new ReactiveProperty<string>();

        public void Update(KomaViewModel other)
        {
            if (other == null)
                return;
            this.Name.Value = other.Name.Value;
            this.IsTransformed.Value = other.IsTransformed.Value;
            this.PlayerType.Value = other.PlayerType.Value;
        }
    }
}
