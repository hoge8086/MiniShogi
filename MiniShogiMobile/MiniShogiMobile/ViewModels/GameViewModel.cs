using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using System.Collections.Generic;

namespace MiniShogiMobile.ViewModels
{
    public class GameViewModel<BoardCell, Hands, HandCell> where BoardCell : CellViewModel, new () where Hands : HandsViewModel<HandCell>, new()  where HandCell : HandKomaViewModel
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

        }

    }

}
