using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Moves
{
    public class StraightKomaMove : IKomaMove
    {
        // [先手の向きで移動可能な位置]
        private RelativeBoardPosition RelativeBoardPosition;
        public StraightKomaMove(RelativeBoardPosition relativeBoardPosition)
        {
            RelativeBoardPosition = relativeBoardPosition;
        }

        public BoardPositions GetMovableBoardPositions(
            Player player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions)
        {
            var positions = new BoardPositions();

            // [後手なら移動可能位置は反転される]
            var toRelativePos = RelativeBoardPosition;
            if (player == Player.SecondPlayer)
                toRelativePos = toRelativePos.Reverse();

            for(var toPos = position.Add(toRelativePos); ; toPos = toPos.Add(toRelativePos))
            {

                // [盤の範囲外に来たら移動不可]
                if (!board.Positions.Contains(toPos))
                    break;

                // [自分の駒に当たったら移動不可]]
                if (turnPlayerKomaPositions.Contains(toPos))
                    break;

                positions.Add(toPos);

                // [相手の駒があったらその先は移動できない]
                if (opponentKomaPositions.Contains(toPos))
                    break;
            }

            return positions;
        }
    }
}
