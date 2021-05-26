using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Komas
{
    public interface IKomaState
    {
        public BoardPositions GetMovableBoardPositions(
            KomaType koma,
            Player player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions);

    }

    public class InHand : IKomaState
    {
        public static readonly InHand State = new InHand();
        private InHand() { }
        public BoardPositions GetMovableBoardPositions(
            KomaType koma,
            Player player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            // [手駒なら空き位置のどこでも置ける]
            var positions = board.Positions;
            positions = positions.Substract(playerKomaPositions);
            positions = positions.Substract(opponentPlayerKomaPositions);
            return positions;
        }
        public override string ToString()
        {
            return "手駒";
        }
    }
    public class OnBoard : IKomaState
    {
        public BoardPosition Position { get; private set; }
        public bool IsTransformed { get;  private set;}

        public OnBoard(BoardPosition position)
        {
            Position = position;
            IsTransformed = false;
        }
        public OnBoard(BoardPosition position, bool isTransformed)
        {
            Position = position;
            IsTransformed = isTransformed;
        }
        public BoardPositions GetMovableBoardPositions(
            KomaType komaType,
            Player player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            return komaType.GetMovableBoardPositions(
                                        player,
                                        Position,
                                        IsTransformed,
                                        board,
                                        playerKomaPositions,
                                        opponentPlayerKomaPositions);
        }

        public override string ToString()
        {
            return Position.ToString() + (IsTransformed ? "@" : "-");
        }
    }

}
