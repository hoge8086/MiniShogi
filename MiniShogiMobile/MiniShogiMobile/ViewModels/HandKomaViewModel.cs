using Prism.Mvvm;
using Reactive.Bindings;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace MiniShogiMobile.ViewModels
{
    public class HandKomaViewModel : BindableBase
    {
        public string Name { get; set; }
        public ReactiveProperty<int> Num { get; set; }
        public PlayerType Player { get; set; }

        public HandKomaViewModel()
        {
            Num = new ReactiveProperty<int>(1);
        }
    }

}
