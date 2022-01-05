namespace MiniShogiMobile.ViewModels
{
    public class GameViewModel<BoardCell, Hands, HandCell> where BoardCell : CellBaseViewModel where Hands : HandViewModel<HandCell>, new()  where HandCell : HandKomaViewModel
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

    }

}
