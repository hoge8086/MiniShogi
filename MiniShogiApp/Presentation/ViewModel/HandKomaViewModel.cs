using Prism.Mvvm;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class HandKomaViewModel : BindableBase, ISelectable
    {
        public string KomaTypeId { get; set; }
        public Player Player { get; set; }
        public string KomaName { get; set; }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }
        public Koma GetKoma(Game game)
        {
            return game.State.FindHandKoma(Player.ToDomain(), KomaTypeId);
        }
    }
}
