using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace MiniShogiMobile.ViewModels
{
    public class HandKomaViewModel : BindableBase
    {
        public KomaTypeId KomaTypeId { get; set; }

        public string Name { get => KomaTypeId.Name; }
        public ReactiveProperty<int> Num { get; set; }
        public PlayerType Player { get; set; }

        public HandKomaViewModel()
        {
            Num = new ReactiveProperty<int>(1);
        }
        public virtual void Clear()
        {
            Num.Value = 0;
        }
    }

}
