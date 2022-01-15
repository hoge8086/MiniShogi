using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public class GameViewModel<BoardCell, Hands, HandCell> where BoardCell : CellViewModel, new () where Hands : HandsViewModel<HandCell>, new()  where HandCell : HandKomaViewModel, new()
    {
        public BoardViewModel<BoardCell> Board { get; set; }
        public Hands HandsOfPlayer1 { get; set; }
        public Hands HandsOfPlayer2 { get; set; }

        public GameViewModel()
        {
            HandsOfPlayer1 = new Hands();
            HandsOfPlayer2 = new Hands();
            Board = new BoardViewModel<BoardCell>();
        }

        public void Update(int height, int width, List<Koma> komaList)
        {
            Board.Update(height, width, komaList);
            HandsOfPlayer1.Update(komaList.Where(x => x.IsInHand && x.Player == PlayerType.Player1).ToList());
            HandsOfPlayer2.Update(komaList.Where(x => x.IsInHand && x.Player == PlayerType.Player2).ToList());
        }

    }

}
